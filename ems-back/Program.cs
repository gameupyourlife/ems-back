﻿using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Minio;
using ems_back;
using ems_back.Repo;
using ems_back.Repo.Data;
using ems_back.Repo.Models;
using ems_back.Repo.Repository;
using ems_back.Repo.MappingProfiles;
using ems_back.Services;
using ems_back.Repo.Services;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Interfaces.Service;
using System.Net;
using Quartz;
using ems_back.Repo.Jobs;
using ems_back.Repo.Jobs.CheckTriggerMethods;
using ems_back.Repo.Jobs.Trigger;
using ems_back.Repo.Jobs.ProcessActionMethods;
using ems_back.Repo.Jobs.Mapping;
using ems_back.Repo.Jobs.Mapping.Actions;
using ems_back.Emails;
using ems_back.Repo.Jobs.Mail;
using ems_back.Repo.Repositories;

namespace ems_back
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Deaktivieren der Zertifikatsüberprüfung in der Entwicklungsumgebung
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();

            // Swagger with JWT support
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "EMS API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by your token.\nExample: Bearer eyJhbGciOi..."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Add CORS services
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                        .WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
                options.AddPolicy("AllowAllOrigins",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

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
            builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            builder.Services.AddScoped<IOrganizationUserRepository, OrganizationUserRepository>();
            builder.Services.AddScoped<IOrganizationDomainRepository, OrganizationDomainRepository>();
            builder.Services.AddScoped<IMailRepository, MailRepository>();
            builder.Services.AddScoped<IMailTemplateRepository, MailTemplateRepository>();

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddScoped<IMailQueueService, MailQueueService>();
            builder.Services.AddScoped<IEventFlowService, EventFlowService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IOrganizationService, OrganizationService>();
            builder.Services.AddScoped<IOrgFlowService, OrgFlowService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IMailRunService, MailRunService>();
            builder.Services.AddScoped<IMailTemplateService, MailTemplateService>();

            // Identity Configuration - supports GUIDs for users and roles
            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddTransient<IRoleStore<IdentityRole<Guid>>, RoleStore<IdentityRole<Guid>, ApplicationDbContext, Guid>>();
            builder.Services.AddScoped<RoleManager<IdentityRole<Guid>>>();

            // JWT Authentication
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

            // Authorization Policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("ADMIN"));
                options.AddPolicy("RequireOwnerRole", policy => policy.RequireRole("OWNER"));
                options.AddPolicy("RequireEventOrganizerRole", policy => policy.RequireRole("EVENT-ORGANIZER"));
                options.AddPolicy("RequireOrganizerRole", policy => policy.RequireRole("ORGANIZER"));
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole("USER"));
            });

            //CheckFlowsJob related services
            builder.Services.AddScoped<MapTriggers>();
            builder.Services.AddScoped<CheckTriggers>();
            builder.Services.AddScoped<MapActions>();
            builder.Services.AddScoped<ProcessActionsForFlow>();

            //Trigger Evaluators
            builder.Services.AddScoped<DateTriggerEvaluator>();
            builder.Services.AddScoped<NumOfAttendeesTriggerEvalator>();
            builder.Services.AddScoped<RelativeDateTriggerEvaluator>();
            builder.Services.AddScoped<StatusTriggerEvaluator>();

            //Action Execution Services
            builder.Services.AddScoped<ChangeTitleExecution>();
            builder.Services.AddScoped<ChangeDescriptionExecution>();
            builder.Services.AddScoped<ChangeImageExecution>();
            builder.Services.AddScoped<ChangeStatusExecution>();
            builder.Services.AddScoped<SendEmailExecution>();

            builder.Services.AddQuartz(opt =>
            {
                opt.SchedulerId = "Scheduler-1";
                opt.SchedulerName = "QuartzScheduler";

                opt.UsePersistentStore(storeOptions =>
                {
                    storeOptions.UseProperties = true;
                    storeOptions.UseBinarySerializer();
                    storeOptions.UsePostgres(postgres =>
                    {
                        postgres.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                    });
                });

                var jobKey = new JobKey("CheckFlowsJob");

                opt.AddJob<CheckFlowsJob>(jobKey, j =>
                {
                    j.WithIdentity(jobKey)
                     .StoreDurably(); // verhindert Duplikate
                });

                opt.AddTrigger(t =>
                {
                    t.ForJob(jobKey)
                     .WithIdentity("CheckFlow", "Default")
                     .WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever());
                });
            });

            builder.Services.AddQuartzHostedService(opt =>
            {
                opt.WaitForJobsToComplete = true;
            });

            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Mail"));
            builder.Services.AddScoped<MailService>();

            builder.Services.AddHostedService<MailQueueWorker>();

            var app = builder.Build();

            // Initialize roles on startup
            using (var scope = app.Services.CreateScope())
            {
                var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();
                await roleService.EnsureRolesExist();
            }

            // Middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}