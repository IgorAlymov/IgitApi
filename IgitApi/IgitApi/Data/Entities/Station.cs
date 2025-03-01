namespace IgitApi.Data.Entities;

public class Station : BaseEntity
{
    public string Name { get; set; } = "";
    public List<EnergyBlock> EnergyBlocks { get; set; } = [];
}
