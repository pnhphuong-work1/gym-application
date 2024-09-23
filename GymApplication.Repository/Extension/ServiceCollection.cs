using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymApplication.Repository.Extension;

public static class ServiceCollection
{
    public static IServiceCollection AddRepository(this IServiceCollection collection, IConfiguration configuration)
    {
        return collection
            .AddDatabase(configuration);
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
}