using ems_back.Repo.Data;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Repository
{
	public class OrganizationRepository : IOrganizationRepository
	{
		private readonly ApplicationDbContext _context;

		public OrganizationRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Organization>> GetAllOrganizationsAsync()
		{
			return await _context.Organizations
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<Organization> GetOrganizationByIdAsync(Guid id)
		{
			return await _context.Organizations
				.FirstOrDefaultAsync(o => o.Id == id);
		}

		public async Task AddOrganizationAsync(Organization organization)
		{
			await _context.Organizations.AddAsync(organization);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateOrganizationAsync(Organization organization)
		{
			_context.Organizations.Update(organization);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteOrganizationAsync(Guid id)
		{
			var organization = await GetOrganizationByIdAsync(id);
			if (organization != null)
			{
				_context.Organizations.Remove(organization);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> OrganizationExistsAsync(Guid id)
		{
			return await _context.Organizations
				.AnyAsync(o => o.Id == id);
		}

		public async Task<IEnumerable<Organization>> GetOrganizationsByUserAsync(Guid userId)
		{
			return await _context.Organizations
				.Where(o => o.Members.Any(m => m.Id == userId))
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<int> GetMemberCountAsync(Guid organizationId)
		{
			return await _context.Users
				.CountAsync(u => u.OrganizationId == organizationId);
		}
	}
}
