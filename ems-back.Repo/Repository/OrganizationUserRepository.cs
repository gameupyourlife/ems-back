using ems_back.Repo.Data;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Repository
{
    public class OrganizationUserRepository : IOrganizationUserRepository
	{
		private readonly ApplicationDbContext _context;

		public OrganizationUserRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<OrganizationUser> GetByIdAsync(Guid id)
		{
			return await _context.OrganizationUsers
				.FirstOrDefaultAsync(ou => ou.Id == id);
		}

		public async Task<IEnumerable<OrganizationUser>> GetByOrganizationIdAsync(Guid organizationId)
		{
			return await _context.OrganizationUsers
				.Where(ou => ou.OrganizationId == organizationId)
				.ToListAsync();
		}

		public async Task<IEnumerable<OrganizationUser>> GetByUserIdAsync(Guid userId)
		{
			return await _context.OrganizationUsers
				.Where(ou => ou.UserId == userId)
				.ToListAsync();
		}

		public async Task<OrganizationUser> GetByOrganizationAndUserIdAsync(Guid organizationId, Guid userId)
		{
			return await _context.OrganizationUsers
				.FirstOrDefaultAsync(ou => ou.OrganizationId == organizationId && ou.UserId == userId);
		}

		public async Task AddAsync(OrganizationUser organizationUser)
		{
			await _context.OrganizationUsers.AddAsync(organizationUser);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(OrganizationUser organizationUser)
		{
			_context.OrganizationUsers.Update(organizationUser);
			await _context.SaveChangesAsync();
		}

		public async Task RemoveAsync(Guid id)
		{
			var organizationUser = await GetByIdAsync(id);
			if (organizationUser != null)
			{
				_context.OrganizationUsers.Remove(organizationUser);
				await _context.SaveChangesAsync();
			}
		}

		public async Task RemoveByOrganizationAndUserIdAsync(Guid organizationId, Guid userId)
		{
			var organizationUser = await GetByOrganizationAndUserIdAsync(organizationId, userId);
			if (organizationUser != null)
			{
				_context.OrganizationUsers.Remove(organizationUser);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> ExistsAsync(Guid organizationId, Guid userId)
		{
			return await _context.OrganizationUsers
				.AnyAsync(ou => ou.OrganizationId == organizationId && ou.UserId == userId);
		}

		public async Task<int> GetMemberCountAsync(Guid organizationId)
		{
			return await _context.OrganizationUsers
				.CountAsync(ou => ou.OrganizationId == organizationId);
		}

		public async Task UpdateUserRoleAsync(Guid organizationId, Guid userId, UserRole newRole)
		{
			var organizationUser = await GetByOrganizationAndUserIdAsync(organizationId, userId);
			if (organizationUser != null)
			{
				organizationUser.UserRole = newRole;
				await UpdateAsync(organizationUser);
			}
		}
	}
}
