using Infrastructure.Common;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Mapping.Mappers;

public class ServiceFeatureMapper : IMapper<ServiceFeatureDAO, ServiceFeature>
{
    public ServiceFeature Map(ServiceFeatureDAO source)
    {
        if (source == null) return null;
        return new ServiceFeature
        {
            Id = source.Id,
            Name = source.Name,
            Value = source.Value,
            ServiceId = source.ServiceId
        };
    }

    public ServiceFeatureDAO MapBack(ServiceFeature target)
    {
        if (target == null) return null;
        return new ServiceFeatureDAO
        {
            Id = target.Id,
            Name = target.Name,
            Value = target.Value,
            ServiceId = target.ServiceId
        };
    }
} 