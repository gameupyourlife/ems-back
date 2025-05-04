using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Interfaces.Repository;

namespace ems_back.Controllers
{
    [Route("api/users")]
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

		// GET: api/users/{userId}
		[HttpGet("{userId}")]
		public async Task<ActionResult<UserResponseDto>> GetUser(Guid userId)
		{
			try
			{
				var user = await _userRepository.GetUserByIdAsync(userId);

				if (user == null)
				{
					_logger.LogWarning("User with id {UserId} not found", userId);
					return NotFound();
				}

				return Ok(user);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting user with id {UserId}", userId);
				return StatusCode(500, "Internal server error");
			}
		}

		// PUT: api/users/{userId}
		[HttpPut("{userId}")]
		public async Task<ActionResult> UpdateUser(Guid userId, [FromBody] UserUpdateDto userDto)
		{
			try
			{
				var updatedUser = await _userRepository.UpdateUserAsync(userId, userDto);
				if (updatedUser == null)
				{
					return NotFound();
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating user with id {UserId}", userId);
				return StatusCode(500, "Internal server error");
			}
		}

		// DELETE: api/users/{usersId}
		[HttpDelete("{userId}")]
		public async Task<IActionResult> DeleteUser(Guid userId)
		{
			try
			{
				var result = await _userRepository.DeleteUserAsync(userId);
				if (!result)
				{
					return NotFound();
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting user with id {UserId}", userId);
				return StatusCode(500, "Internal server error");
			}
		}

        // GET: api/users/{userId}/orgs
        [HttpGet("{userId}/orgs")]
		public async Task<ActionResult<IEnumerable<OrganizationOverviewDto>>> GetUserOrganizations(Guid userId)
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

        // GET: api/users/{userId}/events
        [HttpGet("{userId}/events")]
		public async Task<ActionResult<IEnumerable<EventInfoDTO>>> GetUserEvents(Guid userId)
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
    }
}