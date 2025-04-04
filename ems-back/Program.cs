﻿using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using ems_back.Repo;
using ems_back.Repo.Data;
using ems_back.Repo.Models;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Repository;
using ems_back.Repo.MappingProfiles;

namespace ems_back
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
