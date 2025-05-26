using AutoMapper;
using ems_back.Repo.Data;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Models;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Extensions.Logging;
using AutoMapper.QueryableExtensions;

namespace ems_back.Repo.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(
            ApplicationDbContext context,
            IMapper mapper,
          ILogger<UserRepository> logger,

            UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;

        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto userDto)
        {
            var fullName = $"{userDto.FirstName} {userDto.LastName}";
            _logger.LogInformation("Starting user creation for {FullName} ({Email})", fullName, userDto.Email);

            var user = _mapper.Map<User>(userDto);
            user.UserName = userDto.Email;
            user.CreatedAt = DateTime.UtcNow;
            user.EmailConfirmed = false;
            user.Role = UserRole.User; // Default role

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
            {
                var errorDescriptions = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("User creation failed for {FullName} ({Email}). Errors: {Errors}", fullName, userDto.Email, errorDescriptions);
                throw new ApplicationException(errorDescriptions);
            }
            _logger.LogInformation("User created successfully for {FullName} ({Email}), UserId: {UserId}", fullName, user.Email, user.Id);


            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> UpdateUserAsync(Guid userId, UserUpdateDto userDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update.", userId);
                return null;
            }

            // Allow nulls or updates
            user.ProfilePicture = userDto.ProfilePicture;

            // Update only non-null properties
            if (!string.IsNullOrEmpty(userDto.FirstName))
                user.FirstName = userDto.FirstName;

            if (!string.IsNullOrEmpty(userDto.LastName))
                user.LastName = userDto.LastName;

            await _context.SaveChangesAsync();
            _logger.LogInformation("User with ID {UserId} was successfully updated.", userId);

            // Project to UserResponseDto without needing .Include(u => u.Organization)
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> UpdateUserRoleAsync(UserUpdateRoleDto userDto)
        {
            var user = await _context.Users.FindAsync(userDto.userId);
            if (user == null) return null;

            user.Role = userDto.newRole;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // return await GetUserByIdAsync(userDto.userId);
            // TODO: Ändern,  sobald Funktion tut
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            };
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
            try
            {
                var result = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == id)
                    .ProjectTo<UserResponseDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                if (result != null)
                {
                    _logger.LogInformation("Successfully retrieved user with properties: " +
                                           "Email: {Email}, " +
                                           "FirstName: {FirstName}, " +
                                           "LastName: {LastName}, " +
                                           "FullName: {FullName}",
                        result.Email,
                        result.FirstName,
                        result.LastName,
                        $"{result.FirstName} {result.LastName}");
                }
                else
                {
                    _logger.LogWarning("User with ID {UserId} not found", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
                throw; // Or return null if you prefer to handle errors gracefully
            }
        }

        public async Task<UserResponseDto> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users
               .AsNoTracking()
               .Where(u => u.Email == email)
               .Select(u => new
               {
                   User = u,
                   Organization = _context.Organizations
                       .Where(o => o.Id == _context.OrganizationUsers
                           .Where(ou => ou.UserId == u.Id)
                           .Select(ou => ou.OrganizationId)
                           .FirstOrDefault())
                       .FirstOrDefault()
               })
               .FirstOrDefaultAsync();

            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
               .AsNoTracking()
               .Select(u => new
               {
                   User = u,
                   Organization = _context.Organizations
                       .Where(o => o.Id == _context.OrganizationUsers
                           .Where(ou => ou.UserId == u.Id)
                           .Select(ou => ou.OrganizationId)
                           .FirstOrDefault())
                       .FirstOrDefault()
               })
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
            //var users = await _context.Users
            //    .AsNoTracking()
            //    .Where(u => u.Role == role)
            //    .Select(u => new
            //    {
            //        User = u,
            //        Organization = _context.Organizations
            //             .Where(o => o.Id == _context.OrganizationUsers
            //                 .Where(ou => ou.UserId == u.Id)
            //                 .Select(ou => ou.OrganizationId)
            //                 .FirstOrDefault())
            //             .FirstOrDefault()
            //    })
            //    .ToListAsync();

            //return _mapper.Map<IEnumerable<UserResponseDto>>(users);

            throw new NotImplementedException("GetUsersByRoleAsync is not implemented yet.");
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByOrganizationAsync(Guid organizationId)
        {
            var users = await _context.Users
                .AsNoTracking()
                .Where(u => _context.OrganizationUsers
                    .Any(ou => ou.UserId == u.Id && ou.OrganizationId == organizationId))
                .Select(u => new
                {
                    User = u,
                    Organization = _context.Organizations
                        .Where(o => o.Id == _context.OrganizationUsers
                            .Where(ou => ou.UserId == u.Id)
                            .Select(ou => ou.OrganizationId)
                            .FirstOrDefault())
                        .FirstOrDefault()
                })
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }
        public async Task<IEnumerable<OrganizationDto>> GetUserOrganizationsAsync(Guid userId)
        {

            return await _context.OrganizationUsers
                .Where(ou => ou.UserId == userId)
                .Select(ou => ou.Organization) // Get just the Organization
                .ProjectTo<OrganizationDto>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
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

        public async Task<int> GetNumberOfOwnersAsync(Guid orgId)
        {
            var count = await _context.OrganizationUsers
                 .CountAsync(ou => ou.OrganizationId == orgId && ou.UserRole == UserRole.Owner);
            return count;
        }

        public async Task<int> GetNumberOfOrganizersAsync(Guid orgId)
        {
            var count = await _context.OrganizationUsers
                 .CountAsync(ou => ou.OrganizationId == orgId && ou.UserRole == UserRole.Organizer);
            return count;
        }
    }
}