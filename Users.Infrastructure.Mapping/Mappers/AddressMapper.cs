using Infrastructure.Common;
using Infrastructure.Models;
using Users.Domain.Entities;

namespace Users.Infrastructure.Mapping.Mappers;

public class AddressMapper : BaseMapper<AddressDAO, Address>
{
    public override Address Map(AddressDAO source)
    {
        return SafeMap(source, s => new Address
        {
            Id = s.Id,
            UserId = s.UserId,
            Name = s.Name,
            Latitude = s.Latitude ?? 0,
            Longitude = s.Longitude ?? 0,
            CreatedAt=s.CreatedAt,
            DeletedAt=s.DeletedAt,
            UpdatedAt=s.UpdatedAt
        });
    }

    public override AddressDAO MapBack(Address target)
    {
        return SafeMapBack(target, t => new AddressDAO
        {
            Id = t.Id,
            UserId = t.UserId,
            Name = t.Name,
            Latitude = t.Latitude,
            Longitude = t.Longitude,
            CreatedAt = t.CreatedAt,
            DeletedAt = t.DeletedAt,
            UpdatedAt = t.UpdatedAt
        });
    }
}