using ems_back.Repo.DTOs.Placeholder;
using Microsoft.AspNetCore.Mvc;

namespace ems_back.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        
        public AuthController()
        {
            // Constructor logic if needed
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] PlaceholderDTO loginRequest)
        {
            // Implement login logic here
            throw new NotImplementedException();
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] PlaceholderDTO registerRequest)
        {
            // Implement registration logic here
            throw new NotImplementedException();
        }

    }
}
