using IgitApi.Data.Entities;
using IgitApi.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IgitApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnergyBlockController(IRepository<EnergyBlock> energyBlockRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnergyBlock>>> GetEnergyBlockAsync()
    {
        var energyBlocks = await energyBlockRepository.GetAllAsync();
        return Ok(energyBlocks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EnergyBlock>> GetEnergyBlock(Guid id)
    {
        var energyBlock = await energyBlockRepository.GetByIdAsync(id);
        if (energyBlock == null)
        {
            return NotFound();
        }

        return Ok(energyBlock);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<EnergyBlock>> CreateEnergyBlock(EnergyBlock energyBlock)
    {
        var createdEnergyBlock = await energyBlockRepository.AddAsync(energyBlock);

        return CreatedAtAction(nameof(GetEnergyBlock), new { id = createdEnergyBlock.Id }, Ok(createdEnergyBlock));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")] // Only accessible to Admins
    public async Task<IActionResult> UpdateEnergyBlock(Guid id, EnergyBlock updatedEnergyBlock)
    {
        if (id != updatedEnergyBlock.Id)
        {
            return BadRequest();
        }

        var energyBlockCurrent = await energyBlockRepository.GetByIdAsync(id);
        if (energyBlockCurrent == null)
        {
            return NotFound();
        }

        try
        {
            await energyBlockRepository.UpdateAsync(updatedEnergyBlock);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal Server Error");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteEnergyBlock(Guid id)
    {
        var energyBlock = await energyBlockRepository.GetByIdAsync(id);
        if (energyBlock == null)
        {
            return NotFound();
        }

        await energyBlockRepository.DeleteAsync(id);
        return NoContent();
    }
}
