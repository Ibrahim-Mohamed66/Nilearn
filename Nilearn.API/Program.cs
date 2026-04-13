using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Options;
using Nilearn.API.Hangfire;
using Nilearn.API.Hangfire.Jobs;
using Nilearn.API.Middlewares;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.DependencyInjection;
using Nilearn.Domain.Enums;
using Nilearn.Infrastructure.DependencyInjection;
using Nilearn.Shared.Models;
using Serilog;
namespace Nilearn.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection("AppSettings"));
            builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<AppConfiguration>>().Value);
            var configuration = builder.Configuration.GetSection("AppSettings").Get<AppConfiguration>();

            builder.Host.UseSerilog((hostingContext, services,configuration) =>
            {
                configuration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext()
                .ReadFrom.Services(services);

            });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole(Role.Admin.ToString()));

                options.AddPolicy("AdminOrInstructor", policy =>
                    policy.RequireRole(Role.Admin.ToString(), Role.SuperAdmin.ToString(), Role.Instructor.ToString()));

                options.AddPolicy("InstructorOnly", policy =>
                    policy.RequireRole(Role.Instructor.ToString()));
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(configuration.FrontendUrl)
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

            // Replace the placeholder with the environment variable
            var pgPassword = Environment.GetEnvironmentVariable("PG_PASSWORD") ?? "";
            connectionString = connectionString.Replace("${PG_PASSWORD}", pgPassword);

            builder.Services.AddHangfire(config =>
            {
                config.UsePostgreSqlStorage(options =>
                {
                    options.UseNpgsqlConnection(connectionString);
                });
            });
            
            builder.Services.AddHangfireServer();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddScoped<IEmailJobScheduler,HangfireEmailJobScheduler>();
            builder.Services.AddScoped<IImageJobScheduler, ImageJobScheduler>();
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            //await app.SeedDatabaseAsync();
            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/hangfire");
            app.MapControllers();

            app.Run();
        }
    }
}
