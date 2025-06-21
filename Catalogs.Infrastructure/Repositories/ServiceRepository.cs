using Microsoft.EntityFrameworkCore;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Models;
using AutoMapper;
using Infrastructure.Common;
using Catalogs.Application.DTOs;
using System.Linq.Expressions;
using Catalogs.Infrastructure.Mapping.Mappers;
using FuzzySharp;

namespace Catalogs.Infrastructure.Repositories;

public class ServiceRepository : BaseRepository<Service, ServiceDAO>, IServiceRepository
{
    private readonly ECommerceContext _context;
    private readonly IMapper<ServiceDAO, Service> _mapper;

    public ServiceRepository(ECommerceContext context, IMapper<ServiceDAO, Service> mapper)
        : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Service?> GetById(Guid id)
    {
        var serviceDao = await _context.Services
            .Include(s => s.BaseItem)
            .FirstOrDefaultAsync(s => s.Id == id);

        return serviceDao == null ? null : _mapper.Map(serviceDao);
    }

    public async Task<Service?> GetByIdWithDetails(Guid id)
    {
        var serviceDao = await _context.Services
            .Include(s => s.BaseItem)
                .ThenInclude(bi => bi.Category)
            .Include(s => s.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (serviceDao == null)
            return null;

        return _mapper.Map(serviceDao);
    }

    public async Task<IEnumerable<Service>> GetByCategory(Guid categoryId)
    {
        var services = await _context.Services
            .Include(s => s.BaseItem)
            .Where(s => s.BaseItem.CategoryId == categoryId)
            .ToListAsync();

        return services.Select(s => _mapper.Map(s));
    }

    public async Task<IEnumerable<Service>> GetByBrand(Guid brandId)
    {
        var services = await _context.Services
            .Include(s => s.BaseItem)
            .Include(s => s.Brands)
            .Where(s => s.Brands.Any(b => b.Id == brandId))
            .ToListAsync();

        return services.Select(s => _mapper.Map(s));
    }

    public async Task<IEnumerable<Service>> GetByPriceRange(decimal minPrice, decimal maxPrice)
    {
        var services = await _context.Services
            .Include(s => s.BaseItem)
            .Where(s => s.BaseItem.Price >= (double)minPrice && s.BaseItem.Price <= (double)maxPrice)
            .ToListAsync();

        return services.Select(s => _mapper.Map(s));
    }

    public async Task<IEnumerable<Service>> GetAvailableServices()
    {
        var services = await _context.Services
            .Include(s => s.BaseItem)
            .Where(s => s.BaseItem.IsAvailable)
            .ToListAsync();

        return services.Select(s => _mapper.Map(s));
    }

    public async Task<Service> GetByMediaId(Guid mediaId)
    {
        var service = await _context.Services
            .Include(s => s.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
            .FirstOrDefaultAsync(s => s.BaseItem.ProductMedia.Any(m => m.Id == mediaId));

        if (service == null)
            throw new KeyNotFoundException($"No service found with media ID: {mediaId}");

        return _mapper.Map(service);
    }
    public async Task<IEnumerable<Service>> GetByDurationRange(int minDuration, int maxDuration)
    {
        var services = await _context.Services
            .Include(s => s.BaseItem)
            .Where(s => s.Duration >= minDuration && s.Duration <= maxDuration)
            .ToListAsync();

        return services.Select(s => _mapper.Map(s));
    }

    public async Task<bool> UpdateAsync(Guid id, Service service)
    {
        var serviceDao = await _context.Services
            .Include(s => s.BaseItem)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (serviceDao == null)
            return false;

        // Update BaseItem properties
        serviceDao.BaseItem.Name = service.Name;
        serviceDao.BaseItem.Description = service.Description;
        serviceDao.BaseItem.Price = (double)service.Price;
        serviceDao.BaseItem.CategoryId = service.CategoryId;
        serviceDao.BaseItem.IsAvailable = service.IsAvailable;
        serviceDao.BaseItem.UpdatedAt = DateTime.UtcNow;

        // Update Service specific properties
        serviceDao.Duration = service.Duration;
        serviceDao.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddBrandToServiceAsync(Guid serviceId, Guid brandId)
    {
        var service = await _context.Services.Include(s => s.Brands).FirstOrDefaultAsync(s => s.Id == serviceId && s.DeletedAt == null);
        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == brandId && b.DeletedAt == null);
        if (service == null || brand == null) return false;
        if (service.Brands.Any(b => b.Id == brandId)) return true; // Already associated
        service.Brands.Add(brand);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveBrandFromServiceAsync(Guid serviceId, Guid brandId)
    {
        var service = await _context.Services.Include(s => s.Brands).FirstOrDefaultAsync(s => s.Id == serviceId && s.DeletedAt == null);
        if (service == null) return false;
        var brand = service.Brands.FirstOrDefault(b => b.Id == brandId);
        if (brand == null) return true; // Not associated
        service.Brands.Remove(brand);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Service>> GetServicesByUserIdAsync(Guid userId)
    {
        var services = await _context.Services
            .Include(s => s.BaseItem)
            .Where(s => s.BaseItem.UserId == userId)
            .ToListAsync();
        return services.Select(s => _mapper.Map(s));
    }

    public async Task<IEnumerable<Service>> GetServicesByNameAsync(string name)
    {
        var services = await _context.Services
            .Include(s => s.BaseItem)
            .ToListAsync();
        var results = services
            .Select(s => new { Service = s, Score = Fuzz.Ratio(s.BaseItem.Name, name) })
            .OrderByDescending(x => x.Score)
            .Where(x => x.Score > 60)
            .Select(x => x.Service);
        return results.Select(s => _mapper.Map(s));
    }

    public async Task<IEnumerable<Service>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        var services = await _context.Services
            .Include(s => s.BaseItem)
                .ThenInclude(bi => bi.ProductMedia)
            .Where(s => ids.Contains(s.Id))
            .ToListAsync();
        return services.Select(s => _mapper.Map(s));
    }

    public async Task<Guid?> GetBaseItemIdByServiceIdAsync(Guid serviceId)
    {
        var service = await _context.Services
            .Where(s => s.Id == serviceId && s.DeletedAt == null)
            .Select(s => s.BaseItemId)
            .FirstOrDefaultAsync();

        return service == Guid.Empty ? null : service;
    }
} 