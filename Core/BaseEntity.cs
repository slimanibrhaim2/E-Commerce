using System;

namespace Core;
public abstract class BaseEntity
{
	public Guid _id { get; private set; }
	public DateTime _createdAt { get; private set; }
	public DateTime _updatedAt { get; private set; }
	public DateTime? _deletedAt { get; private set; }

	public BaseEntity()
	{
		this._id = Guid.NewGuid(); 
		InitCreatedAt();
	}
	public void InitCreatedAt()
	{
		this._createdAt = DateTime.Now;
	}
	public void UpdateUpdatedAt()
	{
		this._updatedAt = DateTime.Now;
	}
	public void UpdateDeletedAt()
	{
		this._deletedAt = DateTime.Now;
	}
}
