using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CloudLearningAPI.Interfaces;
using CloudLearningAPI.Services;
using CloudLearningAPI.Repositories;
using Microsoft.AspNetCore.Http;

namespace CloudLearningAPI.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddCors(options => options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
            CookieContainer cookieContainer = new();
            services.AddSingleton(cookieContainer);
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IDropboxService, DropboxService>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddSingleton<IJwtAuthenticationManager, JwtAuthenticationManager>();
            services.AddScoped<IDataParser, DataParser>();
            services.AddHttpClient<SwitchClient>().ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() {
                CookieContainer = cookieContainer,
                UseCookies = true
            });


            return services;
        }
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}