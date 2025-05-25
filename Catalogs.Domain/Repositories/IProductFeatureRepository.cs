using Core.Interfaces;
using Catalogs.Domain.Entities;

namespace Catalogs.Domain.Repositories;

public interface IProductFeatureRepository : IRepository<ProductFeature>
{
    Task<Guid?> AddFeatureAsync(Guid productId, string name, string value);
    Task<bool> UpdateFeatureAsync(Guid featureId, string name, string value);
    Task<bool> DeleteFeatureAsync(Guid featureId);
    Task<ProductFeature?> GetFeatureByIdAsync(Guid featureId);
} 