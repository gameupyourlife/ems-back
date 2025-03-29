using ems_back.Repo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace ems_back.Repo.Data
{
	public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		// Main Entities
		public DbSet<Organization> Organizations { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<EventAttendee> EventAttendees { get; set; }
		public DbSet<AgendaEntry> AgendaEntries { get; set; }
		public DbSet<EventFile> Files { get; set; }
		public DbSet<Flow> Flows { get; set; }
		public DbSet<Trigger> Triggers { get; set; }
		public DbSet<Models.Action> Actions { get; set; } // Renamed from Action to avoid conflict

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configure User-Organization Relationships
			modelBuilder.Entity<User>(b =>
			{
				// Configure the Organization relationship
				b.HasOne(u => u.Organization)
					.WithMany(o => o.Members)
					.HasForeignKey(u => u.OrganizationId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// Configure Organization relationships
			modelBuilder.Entity<Organization>(b =>
			{
				// Creator relationship (User who created the organization)
				b.HasOne(o => o.Creator)
					.WithMany(u => u.CreatedOrganizations)
					.HasForeignKey(o => o.CreatedBy)
					.OnDelete(DeleteBehavior.Restrict);

				// Updater relationship (User who last updated the organization)
				b.HasOne(o => o.Updater)
					.WithMany() // No inverse navigation
					.HasForeignKey(o => o.UpdatedBy)
					.OnDelete(DeleteBehavior.Restrict);

				// Members collection is configured in User configuration above
			});

			// Configure Event relationships
			modelBuilder.Entity<Event>(b =>
			{
				b.HasOne(e => e.Creator)
					.WithMany(u => u.CreatedEvents)
					.HasForeignKey(e => e.CreatedBy)
					.OnDelete(DeleteBehavior.Restrict);

				b.HasOne(e => e.Updater)
					.WithMany() // No inverse navigation
					.HasForeignKey(e => e.UpdatedBy)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// Configure composite key for EventAttendee
			modelBuilder.Entity<EventAttendee>()
				.HasKey(ea => new { ea.EventId, ea.UserId });

			// Configure enum conversions
			modelBuilder.Entity<User>()
				.Property(u => u.Role)
				.HasConversion<string>()
				.HasMaxLength(20);

			modelBuilder.Entity<Event>()
				.Property(e => e.Category)
				.HasConversion<string>();

			modelBuilder.Entity<Event>()
				.Property(e => e.Status)
				.HasConversion<string>();

			// Configure JSON columns (for PostgreSQL)
			modelBuilder.Entity<Trigger>()
				.Property(t => t.Details)
				.HasColumnType("jsonb");

			modelBuilder.Entity<Models.Action>()
				.Property(a => a.Details)
				.HasColumnType("jsonb");
		}
	}
}
