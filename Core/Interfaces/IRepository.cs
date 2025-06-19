using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);

    // for filtering/query
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    void HardDelete(T entity);
}