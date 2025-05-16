using AutoMapper;
using ems_back.Repo.Data;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ems_back.Repo.Repository
{
	public class OrganizationRepository : IOrganizationRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private readonly ILogger<OrganizationRepository> _logger;

		public OrganizationRepository(
			ApplicationDbContext context,
			IMapper mapper,
			ILogger<OrganizationRepository> logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<OrganizationResponseDto> CreateOrganizationAsync(OrganizationCreateDto organizationDto)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				_logger.LogInformation("Creating new organization with name {OrganizationName}", organizationDto.Name);

				var organization = _mapper.Map<Organization>(organizationDto);
				organization.CreatedBy = organizationDto.CreatedBy;
				organization.CreatedAt = DateTime.UtcNow;
				organization.UpdatedAt = DateTime.UtcNow;
				organization.UpdatedBy = organizationDto.CreatedBy;
				organization.ProfilePicture = string.IsNullOrWhiteSpace(organizationDto.ProfilePicture)
					? null
					: organizationDto.ProfilePicture;

				await _context.Organizations.AddAsync(organization);
				await _context.SaveChangesAsync();

				var domain = new OrganizationDomain
				{
					Domain = organizationDto.Domain.ToLower().Trim(),
					OrganizationId = organization.Id,
					Organization = organization
				};

				await _context.OrganizationDomain.AddAsync(domain);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync();

				_logger.LogInformation("Organization created successfully with ID {OrganizationId}", organization.Id);
				return await GetOrganizationByIdAsync(organization.Id);
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				_logger.LogError(ex, "Error creating organization");
				throw;
			}
		}

		public async Task<bool> AddDomainToOrganizationAsync(Guid organizationId, string domain)
		{
			try
			{
				_logger.LogInformation("Adding domain {Domain} to organization {OrganizationId}",
					domain, organizationId);

				if (await _context.OrganizationDomain.AnyAsync(d => d.Domain == domain.ToLower()))
				{
					_logger.LogWarning("Domain {Domain} already exists", domain);
					return false;
				}

				var newDomain = new OrganizationDomain
				{
					Domain = domain.ToLower().Trim(),
					OrganizationId = organizationId
				};

				await _context.OrganizationDomain.AddAsync(newDomain);
				await _context.SaveChangesAsync();

				_logger.LogInformation("Domain {Domain} added successfully to organization {OrganizationId}",
					domain, organizationId);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error adding domain {Domain} to organization {OrganizationId}",
					domain, organizationId);
				throw;
			}
		}

		public async Task<bool> IsDomainAvailableAsync(string domain, Guid? organizationId = null)
		{
			try
			{
				var query = _context.OrganizationDomain
					.Where(d => d.Domain == domain.ToLower());

				if (organizationId.HasValue)
				{
					query = query.Where(d => d.OrganizationId != organizationId.Value);
				}

				var isAvailable = !await query.AnyAsync();
				_logger.LogDebug("Domain {Domain} availability check: {IsAvailable}", domain, isAvailable);
				return isAvailable;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error checking domain availability for {Domain}", domain);
				throw;
			}
		}

		public async Task<bool> DomainExistsAsync(string domain)
		{
			try
			{
				var exists = await _context.OrganizationDomain
					.AnyAsync(d => d.Domain == domain.ToLower());

				_logger.LogDebug("Domain {Domain} exists check: {Exists}", domain, exists);
				return exists;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error checking if domain {Domain} exists", domain);
				throw;
			}
		}

		public async Task<IEnumerable<string>> GetOrganizationDomainsAsync(Guid organizationId)
		{
			try
			{
				var domains = await _context.OrganizationDomain
					.Where(d => d.OrganizationId == organizationId)
					.Select(d => d.Domain)
					.ToListAsync();

				_logger.LogDebug("Retrieved {Count} domains for organization {OrganizationId}",
					domains.Count, organizationId);
				return domains;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving domains for organization {OrganizationId}", organizationId);
				throw;
			}
		}

		public async Task<Organization> GetByIdAsync(
			Guid id,
			Func<IQueryable<Organization>, IQueryable<Organization>> includes = null)
		{
			try
			{
				var query = _context.Organizations.AsQueryable();

				if (includes != null)
				{
					query = includes(query);
				}

				var organization = await query.FirstOrDefaultAsync(e => e.Id == id);

				if (organization == null)
				{
					_logger.LogWarning("Organization with ID {OrganizationId} not found", id);
				}

				return organization;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving organization entity with ID {OrganizationId}", id);
				throw;
			}
		}

		public async Task UpdateAsync(Organization entity)
		{
			try
			{
				_context.Entry(entity).State = EntityState.Modified;
				await _context.SaveChangesAsync();
				_logger.LogInformation("Organization with ID {OrganizationId} updated successfully", entity.Id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating organization with ID {OrganizationId}", entity.Id);
				throw;
			}
		}

		public async Task<bool> DeleteOrganizationAsync(Guid id, Guid updatedByUserId)
		{
			try
			{
				_logger.LogInformation("Deleting organization with ID {OrganizationId}", id);

				var organization = await _context.Organizations.FindAsync(id);
				if (organization == null)
				{
					_logger.LogWarning("Organization with ID {OrganizationId} not found for deletion", id);
					return false;
				}

				_context.Organizations.Remove(organization);
				await _context.SaveChangesAsync();

				_logger.LogInformation("Organization with ID {OrganizationId} deleted successfully", id);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting organization with ID {OrganizationId}", id);
				throw;
			}
		}

		public async Task<OrganizationResponseDto> GetOrganizationByIdAsync(Guid id)
		{
			try
			{
				var organization = await _context.Organizations
					.Include(o => o.Creator)
					.Include(o => o.Updater)
					.AsNoTracking()
					.FirstOrDefaultAsync(o => o.Id == id);

				if (organization == null)
				{
					_logger.LogWarning("Organization with ID {OrganizationId} not found", id);
					return null;
				}

				var dto = _mapper.Map<OrganizationResponseDto>(organization);
				dto.MemberCount = await GetMemberCountAsync(id);

				_logger.LogDebug("Retrieved organization with ID {OrganizationId}", id);
				return dto;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving organization with ID {OrganizationId}", id);
				throw;
			}
		}

		public async Task<IEnumerable<OrganizationResponseDto>> GetAllOrganizationsAsync()
		{
			try
			{
				var organizations = await _context.Organizations
					.Include(o => o.Creator)
					.AsNoTracking()
					.ToListAsync();

				_logger.LogDebug("Retrieved {Count} organizations", organizations.Count);
				return _mapper.Map<IEnumerable<OrganizationResponseDto>>(organizations);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving all organizations");
				throw;
			}
		}

		public async Task<IEnumerable<OrganizationOverviewDto>> GetOrganizationsByUserAsync(Guid userId)
		{
			try
			{
				var organizations = await _context.OrganizationUsers
					.Where(ou => ou.UserId == userId)
					.Select(ou => ou.Organization)
					.Select(o => new OrganizationOverviewDto
					{
						Id = o.Id,
						Name = o.Name,
						ProfilePicture = o.ProfilePicture
					})
					.AsNoTracking()
					.ToListAsync();

				_logger.LogDebug("Retrieved {Count} organizations for user {UserId}",
					organizations.Count, userId);
				return organizations;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving organizations for user {UserId}", userId);
				throw;
			}
		}

		private async Task<int> GetMemberCountAsync(Guid organizationId)
		{
			try
			{
				var count = await _context.OrganizationUsers
					.CountAsync(u => u.OrganizationId == organizationId);

				_logger.LogDebug("Retrieved member count {Count} for organization {OrganizationId}",
					count, organizationId);
				return count;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving member count for organization {OrganizationId}", organizationId);
				throw;
			}
		}

		public async Task<bool> OrganizationExistsAsync(Guid id)
		{
			try
			{
				var exists = await _context.Organizations.AnyAsync(o => o.Id == id);
				_logger.LogDebug("Organization existence check for ID {OrganizationId}: {Exists}", id, exists);
				return exists;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error checking if organization with ID {OrganizationId} exists", id);
				throw;
			}
		}

		public async Task<OrganizationUserDto> GetOrganizationUserAsync(Guid orgId, Guid userId)
        {
            return await _context.OrganizationUsers
                .Where(ou => ou.UserId == userId && ou.OrganizationId == orgId)
                .Select(ou => new OrganizationUserDto
                {
                    UserId = ou.UserId,
                    OrganizationId = ou.OrganizationId,
                    UserRole = ou.UserRole,
                    JoinedAt = ou.JoinedAt
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}