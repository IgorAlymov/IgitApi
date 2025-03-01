namespace IgitApi.Data.Entities;

public class BaseEntity : IBaseEntity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset DateAdded { get; set; }
    public DateTimeOffset DateUpdated { get; set; }
}

public interface IBaseEntity<T>
{
    public T Id { get; set; }
    DateTimeOffset DateAdded { get; set; }
    DateTimeOffset DateUpdated { get; set; }
}
