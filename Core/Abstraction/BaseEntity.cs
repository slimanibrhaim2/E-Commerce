using System;

namespace Core.Abstraction;
public abstract class BaseEntity
{
    public Guid Id { get;  set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public BaseEntity()
    {
        Id = Guid.NewGuid();
        InitCreatedAt();
    }
    public void InitCreatedAt()
    {
        CreatedAt = DateTime.Now;
    }
    public void UpdateUpdatedAt()
    {
        UpdatedAt = DateTime.Now;
    }
    public void UpdateDeletedAt()
    {
        DeletedAt = DateTime.Now;
    }
}
