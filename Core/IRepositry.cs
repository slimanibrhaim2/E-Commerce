namespace Core;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

public interface IRepositry<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(Guid id);
    

}