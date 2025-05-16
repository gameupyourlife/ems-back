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

		public DbSet<Organization> Organizations { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<EventAttendee> EventAttendees { get; set; }
		public DbSet<AgendaEntry> AgendaEntries { get; set; }
		public DbSet<EventFile> Files { get; set; }
		public DbSet<Flow> Flows { get; set; }
		public DbSet<Trigger> Triggers { get; set; }
		public DbSet<Models.Action> Actions { get; set; }
		public DbSet<OrganizationUser> OrganizationUsers { get; set; }
		public DbSet<FlowsRun> FlowsRun { get; set; }
		public DbSet<FlowTemplate> FlowTemplates { get; set; }

        public DbSet<MailTemplate> MailTemplates { get; set; }
        public DbSet<Mail> Mail { get; set; }
        public DbSet<MailRun> MailRun { get; set; }

        public DbSet<OrganizationDomain> OrganizationDomain { get; set; }
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

			// Event relationships:

			modelBuilder.Entity<Event>()
				.HasOne(e => e.Creator)
				.WithMany()
				.HasForeignKey(e => e.CreatedBy)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Event>()
				.HasOne(e => e.Updater)
				.WithMany()
				.HasForeignKey(e => e.UpdatedBy)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Event>()
				.HasOne(e => e.Organization)
				.WithMany(o => o.Events)
				.HasForeignKey(e => e.OrganizationId)
				.OnDelete(DeleteBehavior.Cascade);

			// Flow Relationships:

			modelBuilder.Entity<Flow>()
				.HasOne(f => f.FlowTemplate)
				.WithMany(f => f.Flows)
				.HasForeignKey(f => f.FlowTemplateId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Flow>()
				.HasOne(e => e.Creator)
				.WithMany()
				.HasForeignKey(e => e.CreatedBy)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Flow>()
				.HasOne(e => e.Updater)
				.WithMany()
				.HasForeignKey(e => e.UpdatedBy)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Flow>()
				.HasOne(e => e.Event)
				.WithMany(o => o.Flows)
				.HasForeignKey(e => e.EventId)
				.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Flow>()
				.HasMany(f => f.Actions)
				.WithOne(f => f.Flow)
				.OnDelete(DeleteBehavior.Cascade);

            // FlowRun relationships:

			modelBuilder.Entity<FlowsRun>()
				.HasOne(e => e.Flow)
				.WithMany(o => o.FlowsRuns)
				.HasForeignKey(e => e.FlowId)
				.OnDelete(DeleteBehavior.Cascade);

			// FlowTemplate relationships:

			modelBuilder.Entity<FlowTemplate>()
				.HasOne(e => e.Organization)
				.WithMany(o => o.FlowTemplates)
				.HasForeignKey(e => e.OrganizationId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<FlowTemplate>()
				.HasOne(e => e.Creator)
				.WithMany()
				.HasForeignKey(e => e.CreatedBy)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<FlowTemplate>()
				.HasOne(e => e.Updater)
				.WithMany()
				.HasForeignKey(e => e.UpdatedBy)
				.OnDelete(DeleteBehavior.Restrict);

			// MailTemplate relationships:

			modelBuilder.Entity<MailTemplate>()
				.HasOne(e => e.Organization)
				.WithMany(o => o.MailTemplates)
				.HasForeignKey(e => e.OrganizationId)
				.OnDelete(DeleteBehavior.Cascade);

			// Mail relationships:

			modelBuilder.Entity<Mail>()
				.HasOne(e => e.Event)
				.WithMany(o => o.Mails)
				.HasForeignKey(e => e.EventId)
				.OnDelete(DeleteBehavior.Cascade);

			// MailRun relationships:

			modelBuilder.Entity<MailRun>()
				.HasOne(e => e.Mail)
				.WithMany(o => o.MailRuns)
				.HasForeignKey(e => e.MailId)
				.OnDelete(DeleteBehavior.Cascade);

			// User relationships:

			modelBuilder.Entity<User>()
				.HasMany(e => e.OrganizationUsers)
				.WithOne(ou => ou.User)
				.HasForeignKey(ou => ou.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<User>()
				.HasMany(e => e.AttendedEvents)
				.WithOne(e => e.User)
				.HasForeignKey(e => e.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// Organization relationships:

			modelBuilder.Entity<Organization>(b =>
			{
				b.HasOne(o => o.Creator)
					.WithMany()
					.HasForeignKey(o => o.CreatedBy)
					.OnDelete(DeleteBehavior.Restrict);

				b.HasOne(o => o.Updater)
					.WithMany()
					.HasForeignKey(o => o.UpdatedBy)
					.OnDelete(DeleteBehavior.Restrict);

				b.HasMany(o => o.OrganizationUsers)
					.WithOne(ou => ou.Organization)
					.HasForeignKey(ou => ou.OrganizationId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			// Configure EventAttendee relationships and constraints

			modelBuilder.Entity<EventAttendee>(b =>
			{
				b.HasIndex(ea => new { ea.EventId, ea.UserId }).IsUnique();

				b.HasOne(ea => ea.Event)
					.WithMany(e => e.Attendees)
					.HasForeignKey(ea => ea.EventId)
					.OnDelete(DeleteBehavior.Cascade);

				b.HasOne(ea => ea.User)
					.WithMany(u => u.AttendedEvents)
					.HasForeignKey(ea => ea.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			//Configure OrganizationUser relationships and constraints

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
			   b.HasOne(ou => ou.User)
				   .WithMany(u => u.OrganizationUsers)
				   .HasForeignKey(ou => ou.UserId)
				   .OnDelete(DeleteBehavior.Cascade);
		   });

			// Configure composite key for EventAttendee
			modelBuilder.Entity<EventAttendee>()
				.HasKey(ea => new { ea.EventId, ea.UserId });

			modelBuilder.Entity<OrganizationUser>()
				.HasKey(ea => new { ea.UserId, ea.OrganizationId });

			// Configure enum conversions

			// Configure enum conversions
			modelBuilder.Entity<User>()
				.Property(u => u.Role)
				.HasConversion<string>()
				.HasMaxLength(20);

			modelBuilder.Entity<OrganizationUser>()
				.Property(u => u.UserRole)
				.HasConversion<string>()
				.HasMaxLength(20);

			modelBuilder.Entity<Event>()
				.Property(e => e.Category)
				.HasConversion<string>();

			modelBuilder.Entity<Event>()
				.Property(e => e.Status)
				.HasConversion<string>();

			// Action and Trigger configuration:

			modelBuilder.Entity<Trigger>()
				.Property(t => t.Details)
				.HasColumnType("jsonb");

			modelBuilder.Entity<Models.Action>()
				.Property(a => a.Details)
				.HasColumnType("jsonb");

			modelBuilder.Entity<Models.Action>(entity =>
			{
				entity.HasOne(a => a.Flow)
					  .WithMany(f => f.Actions)
					  .HasForeignKey(a => a.FlowId)
					  .OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(a => a.FlowTemplate)
					  .WithMany(f => f.Actions)
					  .HasForeignKey(a => a.FlowTemplateId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<Trigger>(entity =>
			{
				entity.HasOne(a => a.Flow)
					  .WithMany(f => f.Triggers)
					  .HasForeignKey(a => a.FlowId)
					  .OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(a => a.FlowTemplate)
					  .WithMany(f => f.Triggers)
					  .HasForeignKey(a => a.FlowTemplateId)
					  .OnDelete(DeleteBehavior.Restrict);
			});

			// Configure the relationship, Testing
			modelBuilder.Entity<OrganizationDomain>()
				.HasOne(od => od.Organization)          // Each domain has one organization
				.WithMany(o => o.AllowedDomains)        // Each org has many domains
				.HasForeignKey(od => od.OrganizationId) // Foreign key property
				.OnDelete(DeleteBehavior.Cascade);      // Delete domains when org is deleted

			// Optional: Unique constraint on domain
			modelBuilder.Entity<OrganizationDomain>()
				.HasIndex(od => od.Domain)
				.IsUnique();
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
							 e.Property(nameof(User.ProfilePicture)).IsModified))
							 // || e.Property(nameof(User.Role)).IsModified))
				.Select(e => e.Entity)
				.ToList();
			var changedOrganizations = ChangeTracker.Entries<Organization>()
				.Where(e => e.State == EntityState.Modified &&
				            (e.Property(nameof(Organization.Name)).IsModified ||
				             e.Property(nameof(Organization.Address)).IsModified ||
				             e.Property(nameof(Organization.Description)).IsModified ||
				             e.Property(nameof(Organization.ProfilePicture)).IsModified ||
				             e.Property(nameof(Organization.UpdatedBy)).IsModified 
				            ))
				.Select(e => e.Entity)
				.ToList();

			foreach (var entry in ChangeTracker.Entries<Trigger>())
            {
                var t = entry.Entity;
                if (t.FlowTemplateId == null && t.FlowId == null)
                    throw new InvalidOperationException("Trigger must be assigned to at least a Flow or a FlowTemplate.");
            }

            foreach (var entry in ChangeTracker.Entries<Models.Action>())
            {
                var a = entry.Entity;
                if (a.FlowTemplateId == null && a.FlowId == null)
                    throw new InvalidOperationException("Action must be assigned to at least a Flow or a FlowTemplate.");
            }

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