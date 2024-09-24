using GymApplication.Repository.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymApplication.Repository.Extension;

public static class ServiceCollection
{
    public static IServiceCollection AddRepositoryLayer(this IServiceCollection collection, IConfiguration configuration)
    {
        return collection
            .AddDatabase(configuration)
            .AddIdentity();
    }
    
    private static IServiceCollection AddDatabase(this IServiceCollection collection, IConfiguration configuration)
    {
        collection.AddDbContextPool<ApplicationDbContext>(options =>
        {
            options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors();
        });
        
        return collection;
    }
    
    private static IServiceCollection AddIdentity(this IServiceCollection collection)
    {
        collection.AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                opt.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        collection.Configure<IdentityOptions>(op =>
        {
            op.Lockout.AllowedForNewUsers = true;
            op.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
            op.Lockout.MaxFailedAccessAttempts = 5;
            op.Password.RequiredLength = 6;
            op.Password.RequireDigit = false;
            op.Password.RequireLowercase = false;
            op.Password.RequireUppercase = false;
            op.Password.RequireNonAlphanumeric = false;
        });
        
        return collection;
    }
}