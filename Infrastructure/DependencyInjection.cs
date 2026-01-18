using Application.Contracts.Data;
using Application.Contracts.Providers;
using Infrastructure.Data;
using Infrastructure.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddServices().AddDatabase(configuration);

            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {

            // Add database related services here
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            //services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString));
            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
            services.AddScoped<IPasswordProvider, PasswordProvider>();
            return services;
        }
    }
}
