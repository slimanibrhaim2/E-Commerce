using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogs.Infrastructure.Repositories;

public class ServiceFeatureRepository : BaseRepository<ServiceFeature, ServiceFeatureDAO>, IServiceFeatureRepository
{
    private readonly ECommerceContext _context;
    private readonly IMapper<ServiceFeatureDAO, ServiceFeature> _mapper;

    public ServiceFeatureRepository(ECommerceContext context, IMapper<ServiceFeatureDAO, ServiceFeature> mapper)
        : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ServiceFeature?> GetFeatureByIdAsync(Guid featureId)
    {
        var feature = await _ctx.ServiceFeatures.FindAsync(featureId);
        return feature == null ? null : _mapper.Map(feature);
    }

    public async Task<Guid?> AddFeatureAsync(Guid serviceId, string name, string value)
    {
        var feature = new ServiceFeatureDAO
        {
            Id = Guid.NewGuid(),
            ServiceId = serviceId,
            Name = name,
            Value = value,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeletedAt = null
        };
        _context.ServiceFeatures.Add(feature);
        return feature.Id;
    }

    public async Task<bool> UpdateFeatureAsync(Guid featureId, string name, string value)
    {
        var feature = await _context.ServiceFeatures.FirstOrDefaultAsync(f => f.Id == featureId && f.DeletedAt == null);
        if (feature == null) return false;
        feature.Name = name;
        feature.Value = value;
        feature.UpdatedAt = DateTime.UtcNow;
        return true;
    }

    public async Task<bool> DeleteFeatureAsync(Guid featureId)
    {
        var feature = await _context.ServiceFeatures.FirstOrDefaultAsync(f => f.Id == featureId && f.DeletedAt == null);
        if (feature == null) return false;
        feature.DeletedAt = DateTime.UtcNow;
        return true;
    }
} 