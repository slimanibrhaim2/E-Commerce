using Core.Interfaces;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ECommerceContext _ctx;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(ECommerceContext ctx)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        public async Task BeginTransaction()
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = await _ctx.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("No transaction in progress.");

            try
            {
                await _ctx.SaveChangesAsync();
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                await RollbackTransaction();
                throw;
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransaction()
        {
            if (_currentTransaction == null)
                return;

            try
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _ctx.SaveChangesAsync();
        }

        #region IDisposable Support
        
        public ValueTask DisposeAsync()
        {
            return _ctx.DisposeAsync();
        }

        public void Dispose()
        {
           _ctx.Dispose();
        }
        #endregion
    }
}

