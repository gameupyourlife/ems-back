using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Minio;

using ems_back;
using ems_back.Repo;
using ems_back.Repo.Data;
using ems_back.Repo.Models;
using ems_back.Repo.Repository;
using ems_back.Repo.Services;
using ems_back.Repo.MappingProfiles;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using ems_back.Repo.DTOs.Organization;
using ems_back.Services;

namespace ems_back
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

// ───────────────────────────────────────────────────────────────
// 1) CORS‐Policy definieren (damit Preflight (OPTIONS) klappt)
// ───────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
          .WithOrigins("http://localhost:3000")  // deine React-URL
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});

// ───────────────────────────────────────────────────────────────
// 2) MVC, Swagger, DbContext, AutoMapper
// ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(DbMappingProfile));

// ───────────────────────────────────────────────────────────────
// 3) Repository-Registrierungen
// ───────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthRepository,    AuthRepository>();
builder.Services.AddScoped<IEmailRepository,   EmailRepository>();
builder.Services.AddScoped<IEventRepository,   EventRepository>();
builder.Services.AddScoped<IEventFlowRepository, EventFlowRepository>();
builder.Services.AddScoped<IOrgFlowRepository, OrgFlowRepository>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IUserRepository,    UserRepository>();
builder.Services.AddScoped<IFileRepository,    FileRepository>();

// ───────────────────────────────────────────────────────────────
// 4) Service-Registrierungen
// ───────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService,         AuthService>();
builder.Services.AddScoped<IEmailService,        EmailService>();
builder.Services.AddScoped<IEventService,        EventService>();
builder.Services.AddScoped<IEventFlowService,    EventFlowService>();
builder.Services.AddScoped<IOrgFlowService,      OrgFlowService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IUserService,         UserService>();
builder.Services.AddScoped<ITokenService,        TokenService>();

// ───────────────────────────────────────────────────────────────
// 5) Identity, Authentication & Authorization
// ───────────────────────────────────────────────────────────────
builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager<SignInManager<User>>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = builder.Configuration["Jwt:Issuer"],
        ValidAudience            = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey         = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// ganz WICHTIG: AddAuthorization **nach** AddAuthentication
builder.Services.AddAuthorization();

var app = builder.Build();

// ───────────────────────────────────────────────────────────────
// 6) HTTP‐Pipeline
// ───────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    // zeigt dir Exceptions mit Stacktrace und Swagger
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// CORS **vor** Auth & MapControllers
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
		}
	}

}
