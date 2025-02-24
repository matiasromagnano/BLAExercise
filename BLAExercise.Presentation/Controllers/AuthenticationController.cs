using BLAExercise.Application.DTOs;
using BLAExercise.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BLAExercise.Presentation.Controllers;

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
    public async Task<IActionResult> GetAuthToken(UserLoginDto userDto)
    {
        var authSucceed = await _authenticationService.AuthenticateUser(userDto);

        return authSucceed ? Ok(await _authenticationService.GenerateToken(userDto)) : Unauthorized();
    }
}
