using Infrastructure.Data.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// Các API cơ bản để test BE và kết nối database.
/// </summary>
[ApiController]
[Route("api")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public HealthController(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Ping API – kiểm tra backend đang chạy (không dùng DB).
    /// </summary>
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new
        {
            message = "Pong",
            timestamp = DateTime.UtcNow,
            service = "VSTEP Writing System API"
        });
    }

    /// <summary>
    /// Kiểm tra kết nối MySQL: có kết nối được không, thời gian phản hồi, số bản ghi users (nếu có).
    /// </summary>
    [HttpGet("health/db")]
    public async Task<IActionResult> CheckDatabase(CancellationToken cancellationToken = default)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var canConnect = await _db.Database.CanConnectAsync(cancellationToken);
            if (!canConnect)
            {
                sw.Stop();
                return StatusCode(503, new
                {
                    database = "disconnected",
                    message = "Cannot connect to database",
                    elapsedMs = sw.ElapsedMilliseconds,
                    timestamp = DateTime.UtcNow
                });
            }

            var userCount = await _db.Users.CountAsync(cancellationToken);
            sw.Stop();

            return Ok(new
            {
                database = "connected",
                message = "MySQL connection OK",
                elapsedMs = sw.ElapsedMilliseconds,
                userCount,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            sw.Stop();
            return StatusCode(503, new
            {
                database = "error",
                message = ex.Message,
                elapsedMs = sw.ElapsedMilliseconds,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Thông tin môi trường + DB status (gọn để test nhanh).
    /// </summary>
    [HttpGet("health")]
    public async Task<IActionResult> Health(CancellationToken cancellationToken = default)
    {
        var dbOk = false;
        try
        {
            dbOk = await _db.Database.CanConnectAsync(cancellationToken);
        }
        catch
        {
            // ignore
        }

        return Ok(new
        {
            status = dbOk ? "healthy" : "degraded",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            database = dbOk ? "connected" : "disconnected",
            timestamp = DateTime.UtcNow
        });
    }
}
