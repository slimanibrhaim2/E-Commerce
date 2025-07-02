using System;
using System.Threading.Tasks;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Infrastructure.Models;

namespace Users.Infrastructure.Repositories
{
    public class UserRatingRepository : BaseRepository<UserRating, UserRatingDAO>, IUserRatingRepository
    {
        private readonly ECommerceContext _context;
        private readonly IMapper<UserRatingDAO, UserRating> _mapper;

        public UserRatingRepository(ECommerceContext context, IMapper<UserRatingDAO, UserRating> mapper)
            : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserRating> GetByIdAsync(Guid id)
        {
            var dao = await _context.UserRatings
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null);
            return dao != null ? _mapper.Map(dao) : null;
        }

        public async Task<UserRating> GetByUserIdAsync(Guid userId)
        {
            var dao = await _context.UserRatings
                .FirstOrDefaultAsync(x => x.UserId == userId && x.DeletedAt == null);
            return dao != null ? _mapper.Map(dao) : null;
        }

        public async Task<UserRating> CreateAsync(UserRating userRating)
        {
            var dao = _mapper.MapBack(userRating);
            await _context.UserRatings.AddAsync(dao);
            return _mapper.Map(dao);
        }

        public async Task<UserRating> UpdateAsync(UserRating userRating)
        {
            var existingRating = await GetByIdAsync(userRating.Id);
            if (existingRating == null)
                return null;

            var dao = _mapper.MapBack(userRating);
            _context.Entry(await _context.UserRatings.FindAsync(userRating.Id)).CurrentValues.SetValues(dao);
            return _mapper.Map(dao);
        }

        public async Task DeleteAsync(Guid id)
        {
            var userRating = await GetByIdAsync(id);
            if (userRating != null)
            {
                userRating.DeletedAt = DateTime.UtcNow;
                await UpdateAsync(userRating);
            }
        }
    }
} 