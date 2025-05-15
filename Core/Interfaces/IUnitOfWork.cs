using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable ,IAsyncDisposable
    {
        Task SaveChangesAsync();
        Task BeginTransaction();
        Task RollbackTransaction();
        Task CommitTransaction();
    }
}
