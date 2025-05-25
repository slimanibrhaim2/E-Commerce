using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalogs.Domain.Repositories;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Repositories
{
    public class FeatureRepository : IFeatureRepository
    {
        private readonly ECommerceContext _context;
        public FeatureRepository(ECommerceContext context)
        {
            _context = context;
        }

        public async Task<Guid?> AddFeatureAsync(Guid entityId, string name, string value)
        {
            // Try to add as ProductFeature
            var product = await _context.Products.FindAsync(entityId);
            if (product != null)
            {
                var feature = new ProductFeatureDAO { Id = Guid.NewGuid(), ProductId = entityId, Name = name, Value = value };
                _context.ProductFeatures.Add(feature);
                await _context.SaveChangesAsync();
                return feature.Id;
            }
            // Try to add as ServiceFeature
            var service = await _context.Services.FindAsync(entityId);
            if (service != null)
            {
                var feature = new ServiceFeatureDAO { Id = Guid.NewGuid(), ServiceId = entityId, Name = name, Value = value };
                _context.ServiceFeatures.Add(feature);
                await _context.SaveChangesAsync();
                return feature.Id;
            }
            return null;
        }

        public async Task<bool> UpdateFeatureAsync(Guid featureId, string name, string value)
        {
            var productFeature = await _context.ProductFeatures.FindAsync(featureId);
            if (productFeature != null)
            {
                productFeature.Name = name;
                productFeature.Value = value;
                await _context.SaveChangesAsync();
                return true;
            }
            var serviceFeature = await _context.ServiceFeatures.FindAsync(featureId);
            if (serviceFeature != null)
            {
                serviceFeature.Name = name;
                serviceFeature.Value = value;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteFeatureAsync(Guid featureId)
        {
            var productFeature = await _context.ProductFeatures.FindAsync(featureId);
            if (productFeature != null)
            {
                _context.ProductFeatures.Remove(productFeature);
                await _context.SaveChangesAsync();
                return true;
            }
            var serviceFeature = await _context.ServiceFeatures.FindAsync(featureId);
            if (serviceFeature != null)
            {
                _context.ServiceFeatures.Remove(serviceFeature);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<ProductFeature?> GetProductFeatureByIdAsync(Guid featureId)
        {
            var feature = await _context.ProductFeatures.FindAsync(featureId);
            if (feature == null) return null;
            return new ProductFeature
            {
                Id = feature.Id,
                Name = feature.Name,
                Value = feature.Value,
                DisplayOrder = 0, // Set as needed
                IsActive = true, // Set as needed
                BaseItemId = feature.ProductId
            };
        }

        public async Task<IEnumerable<ProductFeature>> GetProductFeaturesByEntityIdAsync(Guid entityId)
        {
            var features = _context.ProductFeatures.Where(f => f.ProductId == entityId).ToList();
            return features.Select(feature => new ProductFeature
            {
                Id = feature.Id,
                Name = feature.Name,
                Value = feature.Value,
                DisplayOrder = 0, // Set as needed
                IsActive = true, // Set as needed
                BaseItemId = feature.ProductId
            });
        }

        public async Task<ServiceFeature?> GetServiceFeatureByIdAsync(Guid featureId)
        {
            var feature = await _context.ServiceFeatures.FindAsync(featureId);
            if (feature == null) return null;
            return new ServiceFeature
            {
                Id = feature.Id,
                Name = feature.Name,
                Value = feature.Value,
                ServiceId = feature.ServiceId
            };
        }

        public async Task<IEnumerable<ServiceFeature>> GetServiceFeaturesByEntityIdAsync(Guid entityId)
        {
            var features = _context.ServiceFeatures.Where(f => f.ServiceId == entityId).ToList();
            return features.Select(feature => new ServiceFeature
            {
                Id = feature.Id,
                Name = feature.Name,
                Value = feature.Value,
                ServiceId = feature.ServiceId
            });
        }
    }
} 