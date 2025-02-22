using BLAExercise.Core.Services;
using BLAExercise.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BLAExercise.Core.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> GetAuthToken(UserLoginDto user)
        {
            var authSucceed = await _authenticationService.AuthenticateUser(user);

            return authSucceed ? Ok(await _authenticationService.GenerateToken(user)) : Unauthorized();
        }
    }
}
