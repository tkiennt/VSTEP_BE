using Application.DTOs.Dashboard;

namespace Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardStatsResponse> GetUserStatsAsync(int userId);
}
