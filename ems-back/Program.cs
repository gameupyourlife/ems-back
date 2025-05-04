using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ems_back.Repo;
using ems_back.Repo.Data;
using ems_back.Repo.Models;
using ems_back.Repo.Repository;
using ems_back.Repo.MappingProfiles;
using ems_back.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Minio;
using ems_back.Repo.Services;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.DTOs.Organization;
using ems_back.Services;

namespace ems_back
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			// Database Context
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

			// AutoMapper
			builder.Services.AddAutoMapper(typeof(DbMappingProfile));

			// Repositories
			builder.Services.AddScoped<IAuthRepository, AuthRepository>();
			builder.Services.AddScoped<IEmailRepository, EmailRepository>();
			builder.Services.AddScoped<IEventFlowRepository, EventFlowRepository>();
			builder.Services.AddScoped<IOrgFlowRepository, OrgFlowRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
			builder.Services.AddScoped<IEventRepository, EventRepository>();
			builder.Services.AddScoped<IFileRepository, FileRepository>();

			// Services
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<IEmailService, EmailService>();
			builder.Services.AddScoped<IEventFlowService, EventFlowService>();
			builder.Services.AddScoped<IEventService, EventService>();
			builder.Services.AddScoped<IOrganizationService, OrganizationService>();
			builder.Services.AddScoped<IOrgFlowService, OrgFlowService>();
			builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();


            // Replace your current Identity configuration with this:

            // Identity Configuration
            builder.Services.AddIdentityCore<User>(options =>
				{
					// Configure identity options if needed
				})
				.AddRoles<IdentityRole<Guid>>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddSignInManager<SignInManager<User>>()  // Add this line
				.AddDefaultTokenProviders();

			// Add authentication services
			builder.Services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = builder.Configuration["Jwt:Issuer"],
						ValidAudience = builder.Configuration["Jwt:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
					};
				});

			// Authorization
			builder.Services.AddAuthorization();

			// MinIO Configuration
			var minioConfig = builder.Configuration.GetSection("Minio");
			builder.Services.AddMinio(configureClient => configureClient
				.WithEndpoint(minioConfig["Endpoint"])
				.WithCredentials(minioConfig["AccessKey"], minioConfig["SecretKey"])
				.WithSSL()
				.Build());

			// Auth Service
			builder.Services.AddScoped<AuthService>();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();
			app.Run();
		}
	}
}