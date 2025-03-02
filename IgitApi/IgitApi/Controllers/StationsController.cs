using IgitApi.Data.Entities;
using IgitApi.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace IgitApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StationsController(IRepository<Station> stationRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Station>>> GetStations()
    {
        var stations = await stationRepository.GetAllAsync();
        return Ok(stations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Station>> GetStation(Guid id)
    {
        var station = await stationRepository.GetByIdAsync(id);
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
        var createdStation = await stationRepository.AddAsync(station);
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

        var station = await stationRepository.GetByIdAsync(id);
        if (station == null)
        {
            return NotFound();
        }

        try
        {
            await stationRepository.UpdateAsync(updatedStation);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }

        return NoContent();
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> PartialUpdateStation(Guid id, [FromBody] JsonPatchDocument<Station>? patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest("Patch document is invalid.");
        }

        var station = await stationRepository.GetByIdAsync(id);
        if (station == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(station);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await stationRepository.UpdateAsync(station);
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
        var station = await stationRepository.GetByIdAsync(id);
        if (station == null)
        {
            return NotFound();
        }

        await stationRepository.DeleteAsync(id);
        return NoContent();
    }
}
