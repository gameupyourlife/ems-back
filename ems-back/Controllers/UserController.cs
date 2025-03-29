using ems_back.Repo.Interfaces;
using ems_back.Repo.Models;
using Microsoft.AspNetCore.Mvc;

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
		public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
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
		public async Task<ActionResult<User>> GetUserById(Guid id)
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
		public async Task<ActionResult<User>> CreateUser([FromBody] User user)
		{
			try
			{
				// Validate email uniqueness
				if (!await _userRepository.IsEmailUniqueAsync(user.Email))
				{
					return Conflict("Email already exists");
				}

				await _userRepository.AddUserAsync(user);

				// Assuming you have a SaveChangesAsync in your repository or unit of work
				// await _userRepository.SaveAsync(); 

				return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating user");
				return StatusCode(500, "Internal server error");
			}
		}

		// PUT: api/users/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User user)
		{
			try
			{
				if (id != user.Id)
				{
					return BadRequest("ID mismatch");
				}

				// Validate email uniqueness (excluding current user)
				if (!await _userRepository.IsEmailUniqueAsync(user.Email, id))
				{
					return Conflict("Email already exists");
				}

				var existingUser = await _userRepository.GetUserByIdAsync(id);
				if (existingUser == null)
				{
					return NotFound();
				}

				await _userRepository.UpdateUserAsync(user);
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
				var user = await _userRepository.GetUserByIdAsync(id);
				if (user == null)
				{
					return NotFound();
				}

				await _userRepository.DeleteUserAsync(id);
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
		public async Task<ActionResult<User>> GetUserByEmail([FromQuery] string email)
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
		public async Task<ActionResult<IEnumerable<Organization>>> GetUserOrganizations(Guid userId)
		{
			try
			{
				var organizations = await _userRepository.GetUserOrganizationsAsync(userId);
				return Ok(organizations);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
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
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("{userId}/events")]
		public async Task<ActionResult<IEnumerable<Event>>> GetUserEvents(Guid userId)
		{
			try
			{
				var events = await _userRepository.GetUserEventsAsync(userId);
				return Ok(events);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}
