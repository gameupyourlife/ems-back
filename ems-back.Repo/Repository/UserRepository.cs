using ems_back.Repo.Data;

using ems_back.Repo.DTOs;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace ems_back.Repo.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private readonly UserManager<User> _userManager;

		public UserRepository(
			ApplicationDbContext context,
			IMapper mapper,
			UserManager<User> userManager)
		{
			_context = context;
			_mapper = mapper;
			_userManager = userManager;
		}

		public async Task<UserResponseDto> CreateUserAsync(UserCreateDto userDto)
		{
			var user = _mapper.Map<User>(userDto);
			user.UserName = userDto.Email;
			user.CreatedAt = DateTime.UtcNow;
			user.EmailConfirmed = false;
			user.Role = UserRole.Participant; // Default role

			var result = await _userManager.CreateAsync(user, userDto.Password);
			if (!result.Succeeded)
			{
				throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));
			}

			await _context.Entry(user)
				.Reference(u => u.Organization)
				.LoadAsync();

			return _mapper.Map<UserResponseDto>(user);
		}

		public async Task<UserResponseDto> UpdateUserAsync(Guid userId, UserUpdateDto userDto)
		{
			var user = await _context.Users.FindAsync(userId);
			if (user == null) return null;

			// Update only non-null properties
			if (!string.IsNullOrEmpty(userDto.FirstName))
				user.FirstName = userDto.FirstName;
			if (!string.IsNullOrEmpty(userDto.LastName))
				user.LastName = userDto.LastName;
			if (!string.IsNullOrEmpty(userDto.ProfilePicture))
				user.ProfilePicture = userDto.ProfilePicture;

			if (!string.IsNullOrEmpty(userDto.NewPassword))
			{
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				await _userManager.ResetPasswordAsync(user, token, userDto.NewPassword);
			}

			await _context.SaveChangesAsync();
			return await GetUserByIdAsync(userId);
		}

		public async Task<bool> DeleteUserAsync(Guid id)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null) return false;

			_context.Users.Remove(user);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<UserResponseDto> GetUserByIdAsync(Guid id)
		{
			var user = await _context.Users
				.Include(u => u.Organization)
				.AsNoTracking()
				.FirstOrDefaultAsync(u => u.Id == id);

			return user == null ? null : _mapper.Map<UserResponseDto>(user);
		}

		public async Task<UserResponseDto> GetUserByEmailAsync(string email)
		{
			var user = await _context.Users
				.Include(u => u.Organization)
				.AsNoTracking()
				.FirstOrDefaultAsync(u => u.Email == email);

			return user == null ? null : _mapper.Map<UserResponseDto>(user);
		}

		public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
		{
			var users = await _context.Users
				.Include(u => u.Organization)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<UserResponseDto>>(users);
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

		public async Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(UserRole role)
		{
			var users = await _context.Users
				.Where(u => u.Role == role)
				.Include(u => u.Organization)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<UserResponseDto>>(users);
		}

		public async Task<IEnumerable<UserResponseDto>> GetUsersByOrganizationAsync(Guid organizationId)
		{
			var users = await _context.Users
				.Where(u => u.OrganizationId == organizationId)
				.Include(u => u.Organization)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<UserResponseDto>>(users);
		}

		public async Task<IEnumerable<OrganizationDto>> GetUserOrganizationsAsync(Guid userId)
		{
			var organizations = await _context.Users
				.Where(u => u.Id == userId)
				.SelectMany(u => u.Organization.Members.Select(m => m.Organization))
				.Distinct()
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<OrganizationDto>>(organizations);
		}

		public async Task<UserRole> GetUserRoleAsync(Guid userId)
		{
			return await _context.Users
				.Where(u => u.Id == userId)
				.Select(u => u.Role)
				.FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<EventResponseDto>> GetUserEventsAsync(Guid userId)
		{
			var events = await _context.EventAttendees
				.Where(ea => ea.UserId == userId)
				.Select(ea => ea.Event)
				.Include(e => e.Creator)
				.ThenInclude(c => c.Organization)
				.AsNoTracking()
				.ToListAsync();

			return _mapper.Map<IEnumerable<EventResponseDto>>(events);
		}

		// Internal methods for authentication
		public async Task<User> GetUserEntityByIdAsync(Guid id)
		{
			return await _userManager.FindByIdAsync(id.ToString());
		}

		public async Task<User> GetUserEntityByEmailAsync(string email)
		{
			return await _userManager.FindByEmailAsync(email);
		}
	}
}