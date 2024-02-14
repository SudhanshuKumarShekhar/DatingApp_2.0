using DatingApp.Data;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.IRepository;
using DatingApp.Repository;
using DatingApp.Services;
using DatingApp.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(config.GetConnectionString("connectionString"))
            );
            //add core here
            services.AddCors();
            services.AddScoped<ITokenService, TokenService>();
            
            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 
            services.Configure<CloudinarySetting>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddTransient<LogUserActivity>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>();

            return services;
        }
    }
}
