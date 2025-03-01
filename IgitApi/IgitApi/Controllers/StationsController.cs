using IgitApi.Data.Entities;
using IgitApi.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IgitApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StationsController(IRepository<Station> energyBlockRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Station>>> GetStations()
    {
        var stations = await energyBlockRepository.GetAllAsync();
        return Ok(stations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Station>> GetStation(Guid id)
    {
        var station = await energyBlockRepository.GetByIdAsync(id);
        if (station == null)
        {
            return NotFound();
        }

        return Ok(station);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Station>> CreateStation(Station station)
    {
        var createdStation = await energyBlockRepository.AddAsync(station);
        return CreatedAtAction(nameof(GetStation), new { id = createdStation.Id }, Ok(createdStation));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateStation(Guid id, Station updatedStation)
    {
        if (id != updatedStation.Id)
        {
            return BadRequest();
        }

        var station = await energyBlockRepository.GetByIdAsync(id);
        if (station == null)
        {
            return NotFound();
        }

        try
        {
            await energyBlockRepository.UpdateAsync(updatedStation);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteStation(Guid id)
    {
        var station = await energyBlockRepository.GetByIdAsync(id);
        if (station == null)
        {
            return NotFound();
        }

        await energyBlockRepository.DeleteAsync(id);
        return NoContent();
    }
}
