using IgitApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IgitApi.Data.Repositories;

public class StationRepository(IgitDbContext context) : IRepository<Station>
{
    public async Task<IEnumerable<Station>> GetAllAsync() =>
        await context.Stations.Include(s => s.EnergyBlocks).ToListAsync();

    public async Task<Station?> GetByIdAsync(Guid id) =>
        await context.Stations.Include(s => s.EnergyBlocks).FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Station> AddAsync(Station entity)
    {
        context.Stations.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Station entity)
    {
        context.Stations.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var station = await context.Stations.FindAsync(id);
        if (station != null)
        {
            context.Stations.Remove(station);
            await context.SaveChangesAsync();
        }
    }
}
