using ems_back.Repo.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Services.Interfaces;

namespace ems_back.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly ILogger<UsersController> _logger;

		public UsersController(
			IUserService userService,
			ILogger<UsersController> logger)
		{
			_userService = userService;
			_logger = logger;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
		{
			try
			{
				var users = await _userService.GetAllUsersAsync();
				return Ok(users);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting all users");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<UserResponseDto>> GetUserById(Guid id)
		{
			try
			{
				var user = await _userService.GetUserByIdAsync(id);
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

		[HttpPost]
		public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] UserCreateDto userDto)
		{
			try
			{
				var createdUser = await _userService.CreateUserAsync(userDto);
				if (createdUser == null)
				{
					return Conflict("Email already exists");
				}
				return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating user");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto userDto)  
		{
			try
			{
				var updated = await _userService.UpdateUserAsync(id, userDto);
				if (updated == null)
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

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(Guid id)
		{
			try
			{
				var deleted = await _userService.DeleteUserAsync(id);
				if (!deleted)
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

		[HttpGet("by-email")]
		public async Task<ActionResult<UserResponseDto>> GetUserByEmail([FromQuery] string email)
		{
			try
			{
				var user = await _userService.GetUserByEmailAsync(email);
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
				var organizations = await _userService.GetUserOrganizationsAsync(userId);
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
				var role = await _userService.GetUserRoleAsync(userId);
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
				var events = await _userService.GetUserEventsAsync(userId);
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
				var users = await _userService.GetUsersByRoleAsync(role);
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
				var users = await _userService.GetUsersByOrganizationAsync(organizationId);
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