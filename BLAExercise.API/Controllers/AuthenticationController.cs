using AutoMapper;
using BLAExercise.API.Models;
using BLAExercise.Core.Interfaces;
using BLAExercise.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BLAExercise.Core.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public AuthenticationController(IAuthenticationService authenticationService, IMapper mapper)
    {
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> GetAuthToken(UserLoginDto userDto)
    {
        var user = _mapper.Map<User>(userDto);

        var authSucceed = await _authenticationService.AuthenticateUser(user);

        return authSucceed ? Ok(await _authenticationService.GenerateToken(user)) : Unauthorized();
    }
}
