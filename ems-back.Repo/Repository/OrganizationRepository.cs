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

		public async Task<OrganizationResponseDto> CreateOrganizationAsync(OrganizationCreateDto organizationDto)
		{
			var organization = _mapper.Map<Organization>(organizationDto);
			organization.CreatedAt = DateTime.UtcNow;
			organization.UpdatedAt = DateTime.UtcNow;
			organization.UpdatedBy = organizationDto.CreatedBy;

			// Ensure ProfilePicture is nullable (only set if provided)
			if (string.IsNullOrWhiteSpace(organizationDto.ProfilePicture))
			{
				organization.ProfilePicture = null; // Explicitly set to null
			}

			await _context.Organizations.AddAsync(organization);
			await _context.SaveChangesAsync();

			return await GetOrganizationByIdAsync(organization.Id);
		}


		public async Task<OrganizationResponseDto> UpdateOrganizationAsync(Guid id, OrganizationUpdateDto organizationDto)
		{
			var organization = await _context.Organizations.FindAsync(id);
			if (organization == null) return null;

			_mapper.Map(organizationDto, organization);
			organization.UpdatedAt = DateTime.UtcNow;

			await _context.SaveChangesAsync();
			return await GetOrganizationByIdAsync(id);
		}

		public async Task<bool> DeleteOrganizationAsync(Guid id)
		{
			var organization = await _context.Organizations.FindAsync(id);
			if (organization == null) return false;

			_context.Organizations.Remove(organization);
			await _context.SaveChangesAsync();
			return true;
		}

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

		public async Task<IEnumerable<OrganizationResponseDto>> GetAllOrganizationsAsync()
		{
			var organizations = await _context.Organizations
				.Include(o => o.Creator)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<OrganizationResponseDto>>(organizations);
		}

		public async Task<IEnumerable<OrganizationDto>> GetOrganizationsByUserAsync(Guid userId)
		{
            return await _context.OrganizationUsers
				.Where(ou => ou.UserId == userId)
				.Select(ou => ou.Organization)
				.Select(o => new OrganizationDto
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

		public async Task<Organization> GetOrganizationEntityAsync(Guid id)
		{
			return await _context.Organizations.FindAsync(id);
		}
	}
}