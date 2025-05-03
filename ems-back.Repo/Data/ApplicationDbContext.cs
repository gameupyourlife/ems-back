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
		public DbSet<OrganizationUser> OrganizationUsers { get; set; }

		public DbSet<FlowsRun> FlowsRun { get; set; }
		public DbSet<FlowTemplate> FlowTemplates { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//Moved this up 
			modelBuilder.Entity<FlowsRun>()
				.Property(fr => fr.Status)
				.HasConversion<string>(); // Saves enum as string

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

			// Configure OrganizationUser relationships and constraints
			modelBuilder.Entity<OrganizationUser>(b =>
			{
				// Composite unique constraint to prevent duplicate user-org relationships
				b.HasIndex(ou => new { ou.OrganizationId, ou.UserId }).IsUnique();

				// Indexes for performance
				b.HasIndex(ou => ou.UserRole);

				// Relationship with Organization
				b.HasOne(ou => ou.Organization)
					.WithMany(o => o.OrganizationUsers)
					.HasForeignKey(ou => ou.OrganizationId)
					.OnDelete(DeleteBehavior.Cascade);

				// Relationship with User
				//b.HasOne(ou => ou.User)
				//	.WithMany(u => u.OrganizationUsers)
				//	.HasForeignKey(ou => ou.UserId)
				//	.OnDelete(DeleteBehavior.Cascade);
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

			// Sync OrganizationUser data when User or Organization changes
			var changedUsers = ChangeTracker.Entries<User>()
				.Where(e => e.State == EntityState.Modified &&
							(e.Property(nameof(User.FirstName)).IsModified ||
							 e.Property(nameof(User.LastName)).IsModified ||
							 e.Property(nameof(User.Email)).IsModified ||
							 e.Property(nameof(User.ProfilePicture)).IsModified ||
							 e.Property(nameof(User.Role)).IsModified))
				.Select(e => e.Entity)
				.ToList();
			var changedOrganizations = ChangeTracker.Entries<Organization>()
				.Where(e => e.State == EntityState.Modified &&
							(e.Property(nameof(Organization.Name)).IsModified ||
							 e.Property(nameof(Organization.Address)).IsModified ||
							 e.Property(nameof(Organization.Description)).IsModified ||
							 e.Property(nameof(Organization.ProfilePicture)).IsModified ||
							 e.Property(nameof(Organization.Website)).IsModified))
				.Select(e => e.Entity)
				.ToList();

			var result = base.SaveChanges();



			if (changedUsers.Any() || changedOrganizations.Any())
			{
				base.SaveChanges();
			}

			return result;
		}




	}

	// Optional interface for entities with timestamps
	public interface IHasTimestamps
	{
		DateTime UpdatedAt { get; set; }
	}
}