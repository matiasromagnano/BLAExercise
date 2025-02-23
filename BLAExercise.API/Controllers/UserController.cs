using AutoMapper;
using BLAExercise.API.Filters;
using BLAExercise.API.Models;
using BLAExercise.Core.Exceptions;
using BLAExercise.Data.Interfaces;
using BLAExercise.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BLAExercise.API.Controllers;

[Authorize]
[ApiController]
[NormalizeApiResponse]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserController(IMapper mapper, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, int pageSize = 10, string sortBy = nameof(Data.Models.User.Email), bool descendig = true)
    {
        var getQueryParameters = new GetQueryParameters(page, pageSize, sortBy, descendig);

        var user = await _userRepository.GetAsync(getQueryParameters);

        return Ok(user.Select(_mapper.Map<UserDto>));
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        return _mapper.Map<UserDto>(user) is null ? 
            throw new NotFoundException($"User with Id: '{id}' was not found.") 
            : Ok(_mapper.Map<UserDto>(user));
    }

    [HttpGet]
    [Route("GetByEmail")]
    public async Task<IActionResult> GetByEmail([FromQuery][EmailAddress] string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        return _mapper.Map<UserDto>(user) is null ?
            throw new NotFoundException($"User with Email: '{email}' was not found.")
            : Ok(_mapper.Map<UserDto>(user));
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserLoginDto userDto)
    {
        var user = await _userRepository.AddAsync(_mapper.Map<User>(userDto));

        return Created("Created", _mapper.Map<UserDto>(user));
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _userRepository.DeleteAsync(id);

        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] UserDto userToUpdateDto)
    {
        await _userRepository.UpdateAsync(_mapper.Map<User>(userToUpdateDto));

        return NoContent();
    }
}
