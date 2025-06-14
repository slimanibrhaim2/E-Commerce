using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Domain.Entities;

namespace Users.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmail(string email);
        Task<User?> GetByPhoneNumber(string phoneNumber);

        // used by GetUserByIdQuery
        Task<User?> GetByIdWithDetails(Guid id);

        Task<User?> GetByAddressId(Guid addressId);

        Task<IEnumerable<User>> GetUsersByNameAsync(string name);
    }
}
