using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenAiSample.WebApi.Models.Account;
using OpenAiSample.WebApi.Services.Identity;
using OpenApiSample.Data.Entities;

namespace OpenAiSample.WebApi.Features.Users
{
    public class UsersController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly UserManager<User> _userManager;

        public UsersController(IIdentityService identityService,
            UserManager<User> userManager)
        {
            _identityService = identityService;
            _userManager = userManager;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns>
        /// Returns a <see cref="RegisterResponse"/> object.
        /// </returns>
        [HttpPost]
        [Route("sign-up")]
        [ProducesResponseType(typeof(RegisterResponse), 200)]
        [ProducesResponseType(typeof(RegisterResponse), 400)]
        public async Task<IActionResult> RegisterAsync([FromForm] RegisterViewModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Some properties are not valid.");
            }

            var result = await _identityService.RegisterUserAsync(registerModel);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns>
        /// Returns a <see cref="LoginResponse"/> object.
        /// </returns>
        [HttpPost]
        [Route("sign-in")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(LoginResponse), 400)]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginModel)
        {
            var result = await _identityService.LoginUserAsync(loginModel);

            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // Get all users where Role is "TechLead"
        [HttpGet("TechLeads")]
        public async Task<IActionResult> GetTechLeads()
        {
            var techLeads = await _userManager.GetUsersInRoleAsync("TechLead");
            return Ok(techLeads);
        }
    }
}
