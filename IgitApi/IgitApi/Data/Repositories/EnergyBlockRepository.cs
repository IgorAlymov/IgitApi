using IgitApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IgitApi.Data.Repositories;

public class EnergyBlockRepository(IgitDbContext context) : IRepository<EnergyBlock>
{
    public async Task<IEnumerable<EnergyBlock>> GetAllAsync() =>
        await context.EnergyBlocks.ToListAsync();

    public async Task<EnergyBlock?> GetByIdAsync(Guid id) =>
        await context.EnergyBlocks.FirstOrDefaultAsync(s => s.Id == id);

    public async Task AddAsync(EnergyBlock entity)
    {
        context.EnergyBlocks.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EnergyBlock entity)
    {
        context.EnergyBlocks.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var EnergyBlock = await context.EnergyBlocks.FindAsync(id);
        if (EnergyBlock != null)
        {
            context.EnergyBlocks.Remove(EnergyBlock);
            await context.SaveChangesAsync();
        }
    }
}
