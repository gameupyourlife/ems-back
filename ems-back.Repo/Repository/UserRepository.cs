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
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _context;

		public UserRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<User>> GetAllUsersAsync()
		{
			return await _context.Users
				.Include(u => u.Organization)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<User> GetUserByIdAsync(Guid id)
		{
			return await _context.Users
				.Include(u => u.Organization)
				.FirstOrDefaultAsync(u => u.Id == id);
		}

		public async Task<User> GetUserByEmailAsync(string email)
		{
			return await _context.Users
				.Include(u => u.Organization)
				.FirstOrDefaultAsync(u => u.Email == email);
		}

		public async Task AddUserAsync(User user)
		{
			await _context.Users.AddAsync(user);
		}

		public async Task UpdateUserAsync(User user)
		{
			_context.Users.Update(user);
			await Task.CompletedTask;
		}

		public async Task DeleteUserAsync(Guid id)
		{
			var user = await GetUserByIdAsync(id);
			if (user != null)
			{
				_context.Users.Remove(user);
			}
		}

		public async Task<bool> UserExistsAsync(Guid id)
		{
			return await _context.Users.AnyAsync(u => u.Id == id);
		}

		public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null)
		{
			return await _context.Users
				.AllAsync(u => u.Email != email || (excludeUserId != null && u.Id == excludeUserId));
		}

		public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
		{
			return await _context.Users
				.Where(u => u.Role == role)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IEnumerable<User>> GetUsersByOrganizationAsync(Guid organizationId)
		{
			return await _context.Users
				.Where(u => u.OrganizationId == organizationId)
				.Include(u => u.Organization)
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<IEnumerable<Organization>> GetUserOrganizationsAsync(Guid userId)
		{
			return await _context.Users
				.Where(u => u.Id == userId)
				.SelectMany(u => u.Organization.Members.Select(m => m.Organization))
				.Distinct()
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<UserRole> GetUserRoleAsync(Guid userId)
		{
			return await _context.Users
				.Where(u => u.Id == userId)
				.Select(u => u.Role)
				.FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId)
		{
			return await _context.EventAttendees
				.Where(ea => ea.UserId == userId)
				.Select(ea => ea.Event)
				.Include(e => e.Creator)  // Include Creator first
				.ThenInclude(c => c.Organization)  // Then include Organization through Creator
				.Include(e => e.Creator)  // Include Creator again for direct access
				.AsNoTracking()
				.ToListAsync();
		}
	}
}

