using Core.Interfaces;
using Catalogs.Domain.Entities;

namespace Catalogs.Domain.Repositories;

public interface IServiceFeatureRepository : IRepository<ServiceFeature>
{
    Task<Guid?> AddFeatureAsync(Guid serviceId, string name, string value);
    Task<bool> UpdateFeatureAsync(Guid featureId, string name, string value);
    Task<bool> DeleteFeatureAsync(Guid featureId);
    Task<ServiceFeature?> GetFeatureByIdAsync(Guid featureId);
} 