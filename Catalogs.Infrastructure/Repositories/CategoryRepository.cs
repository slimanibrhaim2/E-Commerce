using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogs.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category, CategoryDAO>, ICategoryRepository
{
    private readonly ECommerceContext _context;
    private readonly IMapper<CategoryDAO, Category> _mapper;

    public CategoryRepository(ECommerceContext context, IMapper<CategoryDAO, Category> mapper)
        : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        var categories = _context.Categories.Where(c => c.DeletedAt == null);
        return categories.Select(c => _mapper.Map(c)).ToList();
    }

    public async Task<Guid> AddCategoryAsync(Category category)
    {
        var dao = _mapper.MapBack(category);
        dao.Id = Guid.NewGuid();
        dao.CreatedAt = DateTime.UtcNow;
        dao.UpdatedAt = DateTime.UtcNow;
        dao.DeletedAt = null;
        _context.Categories.Add(dao);
        return dao.Id;
    }

    public async Task<bool> UpdateCategoryAsync(Guid id, Category category)
    {
        var dao = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
        if (dao == null) return false;
        dao.Name = category.Name;
        dao.Description = category.Description;
        dao.UpdatedAt = DateTime.UtcNow;
        dao.ParentId = category.ParentCategoryId;
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var dao = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
        if (dao == null) return false;
        dao.DeletedAt = DateTime.UtcNow;
        return true;
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid id)
    {
        var dao = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
        return dao == null ? null : _mapper.Map(dao);
    }

    public async Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentId)
    {
        var subcategories = _context.Categories.Where(c => c.ParentId == parentId && c.DeletedAt == null);
        return subcategories.Select(c => _mapper.Map(c)).ToList();
    }
} 