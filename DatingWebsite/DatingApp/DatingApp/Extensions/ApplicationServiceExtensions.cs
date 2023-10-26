using DatingApp.Data;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.IRepository;
using DatingApp.Repository;
using DatingApp.Services;
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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); 
            services.Configure<CloudinarySetting>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddTransient<LogUserActivity>();
            services.AddScoped<ILikesRepository, LikeRepository>();

            return services;
        }
    }
}
