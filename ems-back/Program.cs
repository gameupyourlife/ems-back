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
using ems_back.Repo.Services.Interfaces;
using ems_back.Repo.Services;

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
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
			builder.Services.AddScoped<IEventRepository, EventRepository>();
			builder.Services.AddScoped<IFileRepository, FileRepository>();
			builder.Services.AddScoped<IActionRepository, ActionRepository>();
			builder.Services.AddScoped<IAgendaEntryRepository, AgendaEntryRepository>();
			builder.Services.AddScoped<IFlowRepository, FlowRepository>();
			builder.Services.AddScoped<ITriggerRepository, TriggerRepository>();
			builder.Services.AddScoped<IOrganizationUserRepository, OrganizationUserRepository>();
			builder.Services.AddSingleton<MailService>();

			// Services
			builder.Services.AddScoped<IUserService, UserService>();
			builder.Services.AddScoped<IOrganizationService, OrganizationService>();
			builder.Services.AddScoped<ITriggerService, TriggerService>();
			builder.Services.AddScoped<IFlowService, FlowService>();
			builder.Services.AddScoped<IFileService, FileService>();
			builder.Services.AddScoped<IEventService, EventService>();
			
			builder.Services.AddScoped<ITokenService, TokenService>();
			builder.Services.AddScoped<IUserService, UserService>();
			builder.Services.AddScoped<IAuthService, AuthService>();
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