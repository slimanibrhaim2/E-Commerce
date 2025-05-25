using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalogs.Domain.Entities;

namespace Catalogs.Domain.Repositories
{
    public interface IFeatureRepository
    {
        Task<Guid?> AddFeatureAsync(Guid entityId, string name, string value);
        Task<bool> UpdateFeatureAsync(Guid featureId, string name, string value);
        Task<bool> DeleteFeatureAsync(Guid featureId);
        Task<ProductFeature?> GetProductFeatureByIdAsync(Guid featureId);
        Task<IEnumerable<ProductFeature>> GetProductFeaturesByEntityIdAsync(Guid entityId);
        Task<ServiceFeature?> GetServiceFeatureByIdAsync(Guid featureId);
        Task<IEnumerable<ServiceFeature>> GetServiceFeaturesByEntityIdAsync(Guid entityId);
    }
} 