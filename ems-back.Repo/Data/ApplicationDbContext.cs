using ems_back.Repo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using ems_back.Repo.Models.Types;

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
		public DbSet<Flow> Flows { get; set; }
		public DbSet<Trigger> Triggers { get; set; }
		public DbSet<Models.Action> Actions { get; set; }
		public DbSet<OrganizationUser> OrganizationUsers { get; set; }
		public DbSet<FlowsRun> FlowsRun { get; set; }
		public DbSet<FlowTemplate> FlowTemplates { get; set; }
		public DbSet<MailQueueEntry> MailQueueEntries { get; set; }
        public DbSet<MailTemplate> MailTemplates { get; set; }
        public DbSet<Mail> Mail { get; set; }
        public DbSet<MailRun> MailRun { get; set; }
		public DbSet<EventOrganizer> EventOrganizers { get; set; }

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
                .HasMany(e => e.Organizers)
                .WithOne(o => o.Event)
                .HasForeignKey(o => o.EventId)
                .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Event>()
				.HasMany(e => e.Attendees)
                .WithOne(e => e.Event)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
				.HasMany(e => e.Flows)
                .WithOne(f => f.Event)
                .HasForeignKey(f => f.EventId)
                .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Event>()
				.HasMany(e => e.AgendaEntries)
                .WithOne(a => a.Event)
                .HasForeignKey(a => a.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
				.HasMany(e => e.Mails)
                .WithOne(m => m.Event)
                .HasForeignKey(m => m.EventId)
                .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Event>()
				.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Updater)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Flow Relationships:

            modelBuilder.Entity<Flow>()
                .HasMany(f => f.FlowsRuns)
                .WithOne(fr => fr.Flow)
                .HasForeignKey(fr => fr.FlowId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Flow>()
				.HasMany(f => f.Triggers)
                .WithOne(f => f.Flow)
                .HasForeignKey(f => f.FlowId)
                .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Flow>()
				.HasMany(f => f.Actions)
				.WithOne(f => f.Flow)
				.HasForeignKey(f => f.FlowId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Flow>()
                .HasOne(f => f.Creator)
                .WithMany()
                .HasForeignKey(f => f.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Flow>()
                .HasOne(f => f.Updater)
                .WithMany()
                .HasForeignKey(f => f.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // FlowTemplate relationships:

            modelBuilder.Entity<FlowTemplate>()
                .HasMany(e => e.Actions)
                .WithOne(e => e.FlowTemplate)
                .HasForeignKey(e => e.FlowTemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<FlowTemplate>()
                .HasMany(e => e.Triggers)
                .WithOne(e => e.FlowTemplate)
                .HasForeignKey(e => e.FlowTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<FlowTemplate>()
                .HasMany(e => e.Flows)
                .WithOne(e => e.FlowTemplate)
                .HasForeignKey(e => e.FlowTemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<FlowTemplate>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<FlowTemplate>()
                .HasOne(e => e.Updater)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Mail relationships:

            modelBuilder.Entity<Mail>()
				.HasMany(e => e.MailRuns)
                .WithOne(o => o.Mail)
                .HasForeignKey(e => e.MailId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Mail>()
                .HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Mail>()
                .HasOne(e => e.Updater)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // User relationships:

            modelBuilder.Entity<User>()
				.HasMany(e => e.OrganizationUsers)
				.WithOne(ou => ou.User)
				.HasForeignKey(ou => ou.UserId)
				.OnDelete(DeleteBehavior.Restrict); // Wenn User gelöscht wird, wird auch die Verknüpfung zur Org gelöscht

            modelBuilder.Entity<User>()
				.HasMany(e => e.AttendedEvents)
				.WithOne(e => e.User)
				.HasForeignKey(e => e.UserId)
				.OnDelete(DeleteBehavior.Cascade); // Wenn User gelöscht wird, wird auch die Verknüpfung zur Event gelöscht

            modelBuilder.Entity<User>()
				.HasMany(e => e.AssignedEvents)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Wenn User gelöscht wird, wird auch die Verknüpfung als Organizer gelöscht

			// Organization relationships:

			modelBuilder.Entity<Organization>()
				.HasOne(o => o.Creator)
				.WithMany()
				.HasForeignKey(o => o.CreatedBy)
				.OnDelete(DeleteBehavior.SetNull);

			modelBuilder.Entity<Organization>()
				.HasOne(o => o.Updater)
				.WithMany()
				.HasForeignKey(o => o.UpdatedBy)
				.OnDelete(DeleteBehavior.SetNull);

			modelBuilder.Entity<Organization>()
				.HasMany(o => o.MailTemplates)
                .WithOne(o => o.Organization)
                .HasForeignKey(o => o.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Organization>()
				.HasMany(o => o.FlowTemplates)
                .WithOne(o => o.Organization)
                .HasForeignKey(o => o.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Organization>()
                .HasMany(o => o.Events)
                .WithOne(o => o.Organization)
                .HasForeignKey(o => o.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Organization>()
				.HasMany(o => o.AllowedDomains)
                .WithOne(od => od.Organization)
                .HasForeignKey(od => od.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Organization>()
				.HasMany(o => o.OrganizationUsers)
                .WithOne(ou => ou.Organization)
                .HasForeignKey(ou => ou.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Organization>()
				.HasOne(o => o.Creator)
                .WithMany()
                .HasForeignKey(o => o.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Organization>()
                .HasOne(o => o.Updater)
                .WithMany()
                .HasForeignKey(o => o.UpdatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure EventAttendee relationships and constraints

            modelBuilder.Entity<EventAttendee>()
                .HasIndex(ea => new { ea.EventId, ea.UserId }).IsUnique();

			//Configure OrganizationUser relationships and constraints

			modelBuilder.Entity<OrganizationUser>()
				.HasIndex(ou => new { ou.UserId, ou.OrganizationId }).IsUnique();

			modelBuilder.Entity<OrganizationUser>()
				.HasIndex(ou => new { ou.UserId, ou.OrganizationId, ou.UserRole }).IsUnique();

			// Configure composite key for EventAttendee
			modelBuilder.Entity<EventAttendee>()
				.HasKey(ea => new { ea.EventId, ea.UserId });

			modelBuilder.Entity<OrganizationUser>()
				.HasKey(ea => new { ea.UserId, ea.OrganizationId });

            modelBuilder.Entity<EventOrganizer>()
				.HasKey(eo => new { eo.EventId, eo.UserId });

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