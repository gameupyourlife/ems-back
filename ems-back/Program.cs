using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using ems_back.Repo;
using ems_back.Repo.Data;
using ems_back.Repo.Models;
using ems_back.Repo.Repository;
using ems_back.Repo.MappingProfiles;
using Minio;
using Minio.DataModel.Args;
using Microsoft.OpenApi.Models;
using ems_back.Repo.Interfaces.Repository;

namespace ems_back
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddSingleton<MailService>();


            //add Interfaces
            builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            
            builder.Services.AddScoped<IFileRepository, FileRepository>();
            builder.Services.AddScoped<IActionRepository, ActionRepository>();
            builder.Services.AddScoped<IAgendaEntryRepository, AgendaEntryRepository>();
            builder.Services.AddScoped<IFlowRepository, FlowRepository>();
            builder.Services.AddScoped<ITriggerRepository, TriggerRepository>();
            builder.Services.AddScoped<IOrganizationUserRepository, OrganizationUserRepository>();


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add this BEFORE AddIdentity()
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add this to your service configuration
            builder.Services.AddAutoMapper(typeof(DbMappingProfile));

            // Or if you have multiple profiles:
            builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(DbMappingProfile)));

			// Add Identity Services
			builder.Services.AddIdentity<User, IdentityRole<Guid>>()
	            .AddEntityFrameworkStores<ApplicationDbContext>()
	            .AddDefaultTokenProviders();

            builder.Services.AddControllers();

            // Read Minio configuration from appsettings.json
            var minioConfig = builder.Configuration.GetSection("Minio");
            var endpoint = minioConfig["Endpoint"];
            var accessKey = minioConfig["AccessKey"];
            var secretKey = minioConfig["SecretKey"];

            // Add Minio using the custom endpoint and configure additional settings for default MinioClient initialization
            builder.Services.AddMinio(configureClient => configureClient
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .WithSSL()
                .Build());



            var app = builder.Build();
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // ▼ Add this endpoint ▼ (before app.Run())
            /*app.MapGet("/test-db", async (ApplicationDbContext dbContext) =>
            {
	            try
	            {
		            await dbContext.Database.CanConnectAsync();
		            return Results.Ok("Database connection successful!");
	            }
	            catch (Exception ex)
	            {
		            return Results.Problem($"Connection failed: {ex.Message}");
	            }
            });*/


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

			app.Run();

        }
    }
}
