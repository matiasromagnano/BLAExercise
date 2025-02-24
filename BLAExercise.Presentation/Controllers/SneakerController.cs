using BLAExercise.Application.DTOs;
using BLAExercise.Application.Interfaces;
using BLAExercise.Domain.Models;
using BLAExercise.Presentation.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BLAExercise.Presentation.Controllers;

[Authorize]
[ApiController]
[NormalizeApiResponse]
[Route("api/[controller]")]
public class SneakerController : ControllerBase
{
    private readonly ISneakerService _sneakerService;

    public SneakerController(ISneakerService sneakerRepository)
    {
        _sneakerService = sneakerRepository;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var sneaker = await _sneakerService.GetByIdAsync(id);

        return Ok(sneaker);
    }

    [HttpGet]
    [Route("GetByUserId")]
    public async Task<IActionResult> GetByUserId([FromQuery] int userId)
    {
        var sneakers = await _sneakerService.GetByUserIdAsync(userId);

        return Ok(sneakers);
    }

    [HttpGet]
    [Route("GetByUserEmail")]
    public async Task<IActionResult> GetByUserEmail([FromQuery][EmailAddress] string email)
    {
        var sneakers = await _sneakerService.GetByUserEmailAsync(email);

        return Ok(sneakers);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, int pageSize = 10, string sortBy = nameof(Sneaker.Year), bool descendig = true)
    {
        var sneakers = await _sneakerService.GetAsync(page, pageSize, sortBy, descendig);

        return Ok(sneakers);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SneakerCreateDto sneakerDto)
    {
        var sneaker = await _sneakerService.AddAsync(sneakerDto);

        return Created("Created", sneaker);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _sneakerService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] SneakerUpdateDto sneakerToUpdateDto)
    {
        var sneakerUpdated = await _sneakerService.UpdateAsync(sneakerToUpdateDto);

        return Ok(sneakerUpdated);
    }
}
