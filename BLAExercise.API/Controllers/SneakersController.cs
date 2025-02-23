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
public class SneakerController : ControllerBase
{
    private readonly ISneakerRepository _sneakerRepository;
    private readonly IMapper _mapper;

    public SneakerController(ISneakerRepository sneakerRepository, IMapper mapper)
    {
        _sneakerRepository = sneakerRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var sneaker = await _sneakerRepository.GetByIdAsync(id);

        return _mapper.Map<SneakerDto>(sneaker) is null ?
            throw new NotFoundException($"{nameof(Sneaker)} with Id: '{id}' was not found.")
            : Ok(_mapper.Map<SneakerDto>(sneaker));
    }

    [HttpGet]
    [Route("GetByUserId")]
    public async Task<IActionResult> GetByUserId([FromQuery] int userId)
    {
        var sneakers = await _sneakerRepository.GetSneakersByUserIdAsync(userId);

        return sneakers is null || sneakers.Count == 0 ?
            throw new NotFoundException($"{nameof(Sneaker)}s for {nameof(Sneaker.UserId)}: '{userId}' were not found.")
            : Ok(sneakers.Select(_mapper.Map<SneakerDto>));
    }

    [HttpGet]
    [Route("GetByUserEmail")]
    public async Task<IActionResult> GetByUserEmail([FromQuery][EmailAddress] string email)
    {
        var sneakers = await _sneakerRepository.GetSneakersByUserEmailAsync(email);

        return sneakers is null || sneakers.Count == 0 ?
            throw new NotFoundException($"{nameof(Sneaker)}s for {nameof(Data.Models.User.Email)}: '{email}' were not found.")
            : Ok(sneakers.Select(_mapper.Map<SneakerDto>));
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, int pageSize = 10, string sortBy = nameof(Sneaker.Year), bool descendig = true)
    {
        var getQueryParameters = new GetQueryParameters(page, pageSize, sortBy, descendig);

        var sneakers = await _sneakerRepository.GetAsync(getQueryParameters);

        return sneakers is null || sneakers.Count == 0 ?
            throw new NotFoundException($"No {nameof(Sneaker)}s were found.")
            : Ok(sneakers.Select(_mapper.Map<SneakerDto>));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SneakerCreateDto sneakerDto)
    {
        var sneaker = await _sneakerRepository.AddAsync(_mapper.Map<Sneaker>(sneakerDto));

        return Created("Created", _mapper.Map<SneakerDto>(sneaker));
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _sneakerRepository.DeleteAsync(id);

        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] SneakerUpdateDto sneakerToUpdateDto)
    {
        await _sneakerRepository.UpdateAsync(_mapper.Map<Sneaker>(sneakerToUpdateDto));

        return NoContent();
    }
}
