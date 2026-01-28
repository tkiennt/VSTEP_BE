using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces.Repositories;
using Infrastructure.Repositories;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext with MySQL
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, 
                ServerVersion.AutoDetect(connectionString)));

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IExamStructureRepository, ExamStructureRepository>();
        services.AddScoped<ILevelRepository, LevelRepository>();
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<IPartTypeRepository, PartTypeRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IPracticeModeRepository, PracticeModeRepository>();
        services.AddScoped<IPracticeSessionRepository, PracticeSessionRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<IUserSubmissionRepository, UserSubmissionRepository>();

        return services;
    }
}
