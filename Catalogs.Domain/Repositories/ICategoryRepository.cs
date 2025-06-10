using Core.Interfaces;
using Catalogs.Domain.Entities;

namespace Catalogs.Domain.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Guid> AddCategoryAsync(Category category);
    Task<bool> UpdateCategoryAsync(Guid id, Category category);
    Task<bool> DeleteCategoryAsync(Guid id);
    Task<Category?> GetCategoryByIdAsync(Guid id);
    Task<Category?> GetCategoryByNameAsync(string name);
    Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentId);
} 