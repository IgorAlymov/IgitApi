using IgitApi.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IgitApi.Data;

public class IgitDbContext(DbContextOptions options) : IdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<Station> Stations { get; set; }
    public DbSet<EnergyBlock> EnergyBlocks { get; set; }
}
