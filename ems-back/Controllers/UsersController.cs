using Microsoft.AspNetCore.Mvc;
using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Models.Types;
using ems_back.Repo.DTOs.Organization;
using ems_back.Repo.DTOs.User;
using ems_back.Repo.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ems_back.Repo.DTOs.Password;
using ems_back.Repo.Exceptions;

namespace ems_back.Controllers
{
    [Route("api/users")]
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
        private string? GetAuthenticatedUserId()
        {
	        return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

		// GET: api/users/{userId}
		[HttpGet("{userId}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);

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
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid user identity.");
                }

                var updatedUser = await _userService.UpdateUserAsync(userId, userDto);
                if (updatedUser == null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating current user");
                return StatusCode(500, "Internal server error");
            }
        }

		
    

        // DELETE: api/users/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(userId);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (MissingRoleException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with id {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/users/{userId}/orgs
       
        [HttpGet("{userId}/orgs")]
        [ProducesResponseType(typeof(IEnumerable<OrganizationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetUserOrganizations(Guid userId)
        {
	        try
	        {
		        var organizations = await _userService.GetUserOrganizationsAsync(userId);

		        if (organizations == null || !organizations.Any())
		        {
			        _logger.LogInformation("No organizations found for user {UserId}", userId);
			        return NotFound($"No organizations found for user {userId}");
		        }

		        _logger.LogInformation("Successfully retrieved {OrganizationCount} organizations for user {UserId}",
			        organizations.Count(), userId);
		        return Ok(organizations);
	        }
	        catch (KeyNotFoundException ex)
	        {
		        _logger.LogWarning(ex, "User not found: {UserId}", userId);
		        return NotFound(ex.Message);
	        }
	        catch (Exception ex)
	        {
		        _logger.LogError(ex, "Error getting organizations for user {UserId}", userId);
		        return StatusCode(500, "Internal server error while processing your request");
	        }
        }



		// DELETE: api/users/{userId}/admin-delete
		[HttpDelete("UserAccount/delete")]
        public async Task<IActionResult> AdminDeleteUser([FromQuery] Guid? userId, [FromQuery] string? email)
        {
            try
            {
                if (!userId.HasValue && string.IsNullOrEmpty(email))
                {
                    return BadRequest("Either userId or email must be provided.");
                }

                var success = await _userService.DeleteUserByIdOrEmailAsync(userId, email);
                if (!success)
                {
                    return NotFound("User not found or could not be deleted.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin error deleting user by Id or Email");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDto resetDto)
        {
	        try
	        {
		        await _userService.ResetPasswordAsync(resetDto);
		        return Ok(new { Message = "Password reset successful" });
	        }
	        catch (KeyNotFoundException ex)
	        {
		        return NotFound(new { Error = ex.Message });
	        }
	        catch (ArgumentException ex)
	        {
		        return BadRequest(new { Error = ex.Message });
	        }
	        catch (InvalidOperationException ex)
	        {
		        return BadRequest(new { Error = ex.Message });
	        }
	        catch (Exception ex)
	        {
		        return StatusCode(500, new { Error = "An unexpected error occurred" });
	        }
        }
	}
}
