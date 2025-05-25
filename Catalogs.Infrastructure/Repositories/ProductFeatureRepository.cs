using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogs.Infrastructure.Repositories;

public class ProductFeatureRepository : BaseRepository<ProductFeature, ProductFeatureDAO>, IProductFeatureRepository
{
    private readonly ECommerceContext _context;
    private readonly IMapper<ProductFeatureDAO, ProductFeature> _mapper;

    public ProductFeatureRepository(ECommerceContext context, IMapper<ProductFeatureDAO, ProductFeature> mapper)
        : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Guid?> AddFeatureAsync(Guid productId, string name, string value)
    {
        var feature = new ProductFeatureDAO
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = name,
            Value = value,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeletedAt = null
        };
        _context.ProductFeatures.Add(feature);
        await _context.SaveChangesAsync();
        return feature.Id;
    }

    public async Task<bool> UpdateFeatureAsync(Guid featureId, string name, string value)
    {
        var feature = await _context.ProductFeatures.FirstOrDefaultAsync(f => f.Id == featureId && f.DeletedAt == null);
        if (feature == null) return false;
        feature.Name = name;
        feature.Value = value;
        feature.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteFeatureAsync(Guid featureId)
    {
        var feature = await _context.ProductFeatures.FirstOrDefaultAsync(f => f.Id == featureId && f.DeletedAt == null);
        if (feature == null) return false;
        feature.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProductFeature?> GetFeatureByIdAsync(Guid featureId)
    {
        var feature = await _context.ProductFeatures.FirstOrDefaultAsync(f => f.Id == featureId && f.DeletedAt == null);
        return feature == null ? null : _mapper.Map(feature);
    }
} 