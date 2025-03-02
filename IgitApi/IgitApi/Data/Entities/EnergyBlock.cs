namespace IgitApi.Data.Entities;

public class EnergyBlock : BaseEntity
{
    public string Name { get; set; } = "";
    public DateTimeOffset NextMaintenanceDate { get; set; }
    public int SensorCount { get; set; }
    public Guid StationId { get; set; }
    public Station? Station { get; set; }
}
