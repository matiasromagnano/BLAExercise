using BLAExercise.Application.DTOs;
using BLAExercise.Application.Interfaces;
using BLAExercise.Presentation.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BLAExercise.Presentation.Controllers;

[Authorize]
[ApiController]
[NormalizeApiResponse]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, int pageSize = 10, string sortBy = nameof(Domain.Models.User.Email), bool descendig = true)
    {
        var users = await _userService.GetAsync(page, pageSize, sortBy, descendig);

        return Ok(users);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var user = await _userService.GetByIdAsync(id);

        return Ok(user);
    }

    [HttpGet]
    [Route("GetByEmail")]
    public async Task<IActionResult> GetByEmail([FromQuery][EmailAddress] string email)
    {
        var user = await _userService.GetByEmailAsync(email);

        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserLoginDto userLoginDto)
    {
        var user = await _userService.AddAsync(userLoginDto);

        return Created("Created", user);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _userService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] UserUpdateDto userToUpdateDto)
    {
        var updatedUser = await _userService.UpdateAsync(userToUpdateDto);

        return Ok(updatedUser);
    }
}
