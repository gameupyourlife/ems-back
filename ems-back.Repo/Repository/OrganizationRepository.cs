using ems_back.Repo.Data;
using ems_back.Repo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.Interfaces.Repository;

namespace ems_back.Repo.Repository
{
    public class OrganizationRepository : IOrganizationRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;

		public OrganizationRepository(ApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		//only admin
		public async Task<OrganizationResponseDto> CreateOrganizationAsync(OrganizationCreateDto organizationDto)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				// Create the organization
				var organization = _mapper.Map<Organization>(organizationDto);
				organization.CreatedBy = organizationDto.CreatedBy;
				organization.CreatedAt = DateTime.UtcNow;
				organization.UpdatedAt = DateTime.UtcNow;
				organization.UpdatedBy = organizationDto.CreatedBy;

				if (string.IsNullOrWhiteSpace(organizationDto.ProfilePicture))
				{
					organization.ProfilePicture = null;
				}

				await _context.Organizations.AddAsync(organization);
				await _context.SaveChangesAsync();

				// Create the domain entry
				var domain = new OrganizationDomain
				{
					Domain = organizationDto.Domain.ToLower().Trim(),
					OrganizationId = organization.Id,
					Organization = organization
				};

				await _context.OrganizationDomain.AddAsync(domain);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync();

				return await GetOrganizationByIdAsync(organization.Id);
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		// Add this new method to handle domain creation
		public async Task<bool> AddDomainToOrganizationAsync(Guid organizationId, string domain)
		{
			var existingDomain = await _context.OrganizationDomain
				.FirstOrDefaultAsync(d => d.Domain == domain.ToLower());

			if (existingDomain != null)
			{
				return false; // Domain already exists
			}

			var newDomain = new OrganizationDomain
			{
				Domain = domain.ToLower().Trim(),
				OrganizationId = organizationId
			};

			await _context.OrganizationDomain.AddAsync(newDomain);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> IsDomainAvailableAsync(string domain, Guid? organizationId = null)
		{
			var query = _context.OrganizationDomain
				.Where(d => d.Domain == domain.ToLower());

			if (organizationId.HasValue)
			{
				query = query.Where(d => d.OrganizationId != organizationId.Value);
			}

			return !await query.AnyAsync();
		}

		public async Task<bool> DomainExistsAsync(string domain)
		{
			return await _context.OrganizationDomain
				.AnyAsync(d => d.Domain == domain.ToLower());
		}



		// Add this method to get domains for an organization
		public async Task<IEnumerable<string>> GetOrganizationDomainsAsync(Guid organizationId)
		{
			return await _context.OrganizationDomain
				.Where(d => d.OrganizationId == organizationId)
				.Select(d => d.Domain)
				.ToListAsync();
		}


		//only admin or Owner
		public async Task<Organization> GetByIdAsync(
			Guid id,
			Func<IQueryable<Organization>, IQueryable<Organization>> includes = null)
		{
			var query = _context.Organizations.AsQueryable();

			if (includes != null)
			{
				query = includes(query);
			}

			return await query.FirstOrDefaultAsync(e => e.Id == id);
		}

		public async Task UpdateAsync(Organization entity)
		{
			_context.Entry(entity).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}



		//only admin or Owner
		public async Task<bool> DeleteOrganizationAsync(Guid id, Guid updatedByUserId)
		{
			var organization = await _context.Organizations.FindAsync(id);
			if (organization == null) return false;

			_context.Organizations.Remove(organization);
			await _context.SaveChangesAsync();
			return true;
		}

		//Free for everyone
		public async Task<OrganizationResponseDto> GetOrganizationByIdAsync(Guid id)
		{
			var organization = await _context.Organizations
				.Include(o => o.Creator)
				.Include(o => o.Updater)
				.AsNoTracking()
				.FirstOrDefaultAsync(o => o.Id == id);

			if (organization == null) return null;

			var dto = _mapper.Map<OrganizationResponseDto>(organization);
			dto.MemberCount = await GetMemberCountAsync(id);

			return dto;
		}

		//only Admin should have this right to do this
		public async Task<IEnumerable<OrganizationResponseDto>> GetAllOrganizationsAsync()
		{
			var organizations = await _context.Organizations
				.Include(o => o.Creator)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<OrganizationResponseDto>>(organizations);
		}

		public async Task<IEnumerable<OrganizationOverviewDto>> GetOrganizationsByUserAsync(Guid userId)
		{
            return await _context.OrganizationUsers
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
        }

		public async Task<int> GetMemberCountAsync(Guid organizationId)
		{
			return await _context.OrganizationUsers
				.CountAsync(u => u.OrganizationId == organizationId);
		}

		public async Task<bool> OrganizationExistsAsync(Guid id)
		{
			return await _context.Organizations.AnyAsync(o => o.Id == id);
		}

	}
}