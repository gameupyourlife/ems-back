using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using ems_back.Repo.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace ems_back.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepository _userRepository;
		private readonly ILogger<UsersController> _logger;

		public UsersController(
			IUserRepository userRepository,
			ILogger<UsersController> logger)
		{
			_userRepository = userRepository;
			_logger = logger;
		}

		// GET: api/users
		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
		{
			try
			{
				var users = await _userRepository.GetAllUsersAsync();
				return Ok(users);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting all users");
				return StatusCode(500, "Internal server error");
			}
		}

		// GET: api/users/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<UserResponseDto>> GetUserById(Guid id)
		{
			try
			{
				var user = await _userRepository.GetUserByIdAsync(id);

				if (user == null)
				{
					_logger.LogWarning("User with id {UserId} not found", id);
					return NotFound();
				}

				return Ok(user);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting user with id {UserId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		// POST: api/users
		[HttpPost]
		public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] UserCreateDto userDto)
		{
			try
			{
				// Validate email uniqueness
				if (!await _userRepository.IsEmailUniqueAsync(userDto.Email))
				{
					return Conflict("Email already exists");
				}

				var createdUser = await _userRepository.CreateUserAsync(userDto);
				return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating user");
				return StatusCode(500, "Internal server error");
			}
		}

		// PUT: api/users/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto userDto)
		{
			try
			{
				var updatedUser = await _userRepository.UpdateUserAsync(id, userDto);
				if (updatedUser == null)
				{
					return NotFound();
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating user with id {UserId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		// DELETE: api/users/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(Guid id)
		{
			try
			{
				var result = await _userRepository.DeleteUserAsync(id);
				if (!result)
				{
					return NotFound();
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting user with id {UserId}", id);
				return StatusCode(500, "Internal server error");
			}
		}

		// GET: api/users/by-email?email=test@example.com
		[HttpGet("by-email")]
		public async Task<ActionResult<UserResponseDto>> GetUserByEmail([FromQuery] string email)
		{
			try
			{
				var user = await _userRepository.GetUserByEmailAsync(email);
				if (user == null)
				{
					return NotFound();
				}
				return Ok(user);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting user by email {Email}", email);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("{userId}/organizations")]
		public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetUserOrganizations(Guid userId)
		{
			try
			{
				var organizations = await _userRepository.GetUserOrganizationsAsync(userId);
				return Ok(organizations);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting organizations for user {UserId}", userId);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("{userId}/role")]
		public async Task<ActionResult<UserRole>> GetUserRole(Guid userId)
		{
			try
			{
				var role = await _userRepository.GetUserRoleAsync(userId);
				return Ok(role);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting role for user {UserId}", userId);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("{userId}/events")]
		public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetUserEvents(Guid userId)
		{
			try
			{
				var events = await _userRepository.GetUserEventsAsync(userId);
				return Ok(events);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting events for user {UserId}", userId);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("by-role/{role}")]
		public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsersByRole(UserRole role)
		{
			try
			{
				var users = await _userRepository.GetUsersByRoleAsync(role);
				return Ok(users);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting users by role {Role}", role);
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("by-organization/{organizationId}")]
		public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsersByOrganization(Guid organizationId)
		{
			try
			{
				var users = await _userRepository.GetUsersByOrganizationAsync(organizationId);
				return Ok(users);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting users by organization {OrganizationId}", organizationId);
				return StatusCode(500, "Internal server error");
			}
		}
	}
}