using ems_back.Repo.Data;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ems_back.Repo.Repository
{
	public class OrganizationUserRepository : IOrganizationUserRepository
	{
		private readonly ApplicationDbContext _context;

		public OrganizationUserRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<OrganizationUser?> GetAsync(Guid userId, Guid organizationId)
		{
			return await _context.OrganizationUsers
				.Include(ou => ou.Organization)
				.Include(ou => ou.User)
				.FirstOrDefaultAsync(ou => ou.UserId == userId && ou.OrganizationId == organizationId);
		}

		public async Task AddAsync(OrganizationUser membership)
		{
			if (membership == null)
				throw new ArgumentNullException(nameof(membership));

			await _context.OrganizationUsers.AddAsync(membership);
			await _context.SaveChangesAsync();
		}

		public async Task RemoveAsync(OrganizationUser membership)
		{
			if (membership == null)
				throw new ArgumentNullException(nameof(membership));

			_context.OrganizationUsers.Remove(membership);
			await _context.SaveChangesAsync();
		}

		public async Task<bool> ExistsAsync(Guid userId, Guid organizationId)
		{
			return await _context.OrganizationUsers
				.AnyAsync(ou => ou.UserId == userId && ou.OrganizationId == organizationId);
		}
	}
}