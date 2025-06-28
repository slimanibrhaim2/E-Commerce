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
            BaseItemId = source.BaseItemId,
            Price = (source.BaseItem?.Price ?? 0),
            CategoryId = source.BaseItem?.CategoryId ?? Guid.Empty,
            IsAvailable = source.BaseItem?.IsAvailable ?? false,
            UserId = source.BaseItem?.UserId ?? Guid.Empty,
            Features = source.ServiceFeatures?.Select(f => new ServiceFeature
            {
                Id = f.Id,
                Name = f.Name,
                Value = f.Value,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            }).ToList() ?? new List<ServiceFeature>(),
            Media = source.BaseItem?.ProductMedia?.Select(m => new Media
            {
                Id = m.Id,
                MediaUrl = m.MediaUrl,
                MediaTypeId = m.MediaTypeId,
                BaseItemId = m.BaseItemId,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            }).ToList() ?? new List<Media>(),
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }

    public ServiceDAO MapBack(Service target)
    {
        if (target == null) return null;
        var serviceDao = new ServiceDAO
        {
            Id = target.Id,
            ServiceType = target.ServiceType,
            Duration = target.Duration,
            BaseItemId = target.BaseItemId,
            BaseItem = new BaseItemDAO
            {
                Id = target.BaseItemId,
                Name = target.Name,
                Description = target.Description,
                Price = (double)target.Price,
                CategoryId = target.CategoryId,
                IsAvailable = target.IsAvailable,
                UserId = target.UserId,
                ProductMedia = target.Media?.Select(m => new MediaDAO
                {
                    Id = m.Id,
                    MediaUrl = m.MediaUrl,
                    MediaTypeId = m.MediaTypeId,
                    BaseItemId = target.BaseItemId,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList() ?? new List<MediaDAO>()
            },
            ServiceFeatures = target.Features?.Select(f => new ServiceFeatureDAO
            {
                Id = f.Id,
                Name = f.Name,
                Value = f.Value,
                ServiceId = target.Id,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            }).ToList() ?? new List<ServiceFeatureDAO>(),
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt
        };
        return serviceDao;
    }
} 