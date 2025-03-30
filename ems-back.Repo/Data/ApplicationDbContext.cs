using ems_back.Repo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace ems_back.Repo.Data
{
	public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
			if (Database.IsNpgsql())
			{
				AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
			}
		}

		// Main Entities
		public DbSet<Organization> Organizations { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<EventAttendee> EventAttendees { get; set; }
		public DbSet<AgendaEntry> AgendaEntries { get; set; }
		public DbSet<EventFile> Files { get; set; } // Will be excluded from auto-ID generation
		public DbSet<Flow> Flows { get; set; }
		public DbSet<Trigger> Triggers { get; set; }
		public DbSet<Models.Action> Actions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Global configuration for all entities
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				// Auto UUID for primary keys (except EventFile)
				if (entityType.ClrType != typeof(EventFile) &&
					entityType.FindProperty("Id") is { } idProperty &&
					idProperty.ClrType == typeof(Guid))
				{
					idProperty.SetDefaultValueSql("gen_random_uuid()");
				}

				// Auto timestamp for CreatedAt
				if (entityType.FindProperty("CreatedAt") is { } createdAtProperty)
				{
					createdAtProperty.SetDefaultValueSql("CURRENT_TIMESTAMP");
					// Either:
					createdAtProperty.ValueGenerated = ValueGenerated.OnAdd;
					// Or:
					modelBuilder.Entity(entityType.ClrType)
						.Property("CreatedAt")
						.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
				}

				// Auto-update UpdatedAt
				if (entityType.FindProperty("UpdatedAt") is { } updatedAtProperty)
				{
					updatedAtProperty.SetDefaultValueSql("CURRENT_TIMESTAMP");
				}
			}


			// Special handling for EventFile (no auto UUID)
			modelBuilder.Entity<EventFile>(b =>
			{
				b.Property(f => f.Id).HasDefaultValueSql(null);
				// Add other file-specific configurations if needed
			});

			// Configure User relationships
			modelBuilder.Entity<User>(b =>
			{
				b.HasOne(u => u.Organization)
					.WithMany(o => o.Members)
					.HasForeignKey(u => u.OrganizationId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// Configure Organization relationships
			modelBuilder.Entity<Organization>(b =>
			{
				b.HasOne(o => o.Creator)
					.WithMany(u => u.CreatedOrganizations)
					.HasForeignKey(o => o.CreatedBy)
					.OnDelete(DeleteBehavior.Restrict);

				b.HasOne(o => o.Updater)
					.WithMany()
					.HasForeignKey(o => o.UpdatedBy)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// Configure Event relationships
			modelBuilder.Entity<Event>(b =>
			{
				b.HasOne(e => e.Creator)
					.WithMany(u => u.CreatedEvents)
					.HasForeignKey(e => e.CreatedBy)
					.OnDelete(DeleteBehavior.Restrict);

				b.HasOne(e => e.Updater)
					.WithMany()
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

			// Configure JSON columns for PostgreSQL
			modelBuilder.Entity<Trigger>()
				.Property(t => t.Details)
				.HasColumnType("jsonb");

			modelBuilder.Entity<Models.Action>()
				.Property(a => a.Details)
				.HasColumnType("jsonb");
		}

		public override int SaveChanges()
		{
			// Ensure UpdatedAt is set on modification
			foreach (var entry in ChangeTracker.Entries()
				.Where(e => e.State == EntityState.Modified &&
						   e.Entity is IHasTimestamps))
			{
				((IHasTimestamps)entry.Entity).UpdatedAt = DateTime.UtcNow;
			}
			return base.SaveChanges();
		}
	}

	// Optional interface for entities with timestamps
	public interface IHasTimestamps
	{
		DateTime UpdatedAt { get; set; }
	}
}