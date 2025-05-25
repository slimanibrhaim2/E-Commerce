using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogs.Infrastructure.Repositories;

public class BrandRepository : BaseRepository<Brand, BrandDAO>, IBrandRepository
{
    private readonly ECommerceContext _context;
    private readonly IMapper<BrandDAO, Brand> _mapper;

    public BrandRepository(ECommerceContext context, IMapper<BrandDAO, Brand> mapper)
        : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
    {
        var brands = await _context.Brands.ToListAsync();
        return brands.Select(b => _mapper.Map(b));
    }

    public async Task<Guid> AddBrandAsync(Brand brand)
    {
        var dao = _mapper.MapBack(brand);
        dao.Id = Guid.NewGuid();
        dao.CreatedAt = DateTime.UtcNow;
        dao.UpdatedAt = DateTime.UtcNow;
        dao.DeletedAt = null;
        _context.Brands.Add(dao);
        await _context.SaveChangesAsync();
        return dao.Id;
    }

    public async Task<bool> UpdateBrandAsync(Guid id, Brand brand)
    {
        var dao = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);
        if (dao == null) return false;
        dao.Name = brand.Name;
        dao.Description = brand.Description;
        dao.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBrandAsync(Guid id)
    {
        var dao = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);
        if (dao == null) return false;
        dao.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Brand?> GetBrandByIdAsync(Guid id)
    {
        var dao = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);
        return dao == null ? null : _mapper.Map(dao);
    }
} 