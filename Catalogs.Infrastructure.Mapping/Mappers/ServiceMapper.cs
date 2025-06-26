using Infrastructure.Common;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Mapping.Mappers;

public class ServiceMapper : IMapper<ServiceDAO, Service>
{
    public Service Map(ServiceDAO source)
    {
        if (source == null) return null;
        return new Service
        {
            Id = source.Id,
            ServiceType = source.ServiceType,
            Duration = source.Duration,
            Name = source.BaseItem?.Name,
            Description = source.BaseItem?.Description,
            Price = (source.BaseItem?.Price ?? 0),
            CategoryId = source.BaseItem?.CategoryId ?? Guid.Empty,
            IsAvailable = source.BaseItem?.IsAvailable ?? false,
            UserId = source.BaseItem?.UserId ?? Guid.Empty
            // Media, Favorites, etc. can be mapped if needed
        };
    }

    public ServiceDAO MapBack(Service target)
    {
        if (target == null) return null;
        return new ServiceDAO
        {
            Id = target.Id,
            ServiceType = target.ServiceType,
            Duration = target.Duration,
            BaseItem = new BaseItemDAO
            {
                Name = target.Name,
                Description = target.Description,
                Price = (double)target.Price,
                CategoryId = target.CategoryId,
                IsAvailable = target.IsAvailable,
                UserId = target.UserId
            }
        };
    }
} 