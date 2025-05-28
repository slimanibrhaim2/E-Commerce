using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Communication.Infrastructure.Repositories
{
    public class CommentRepository : BaseRepository<Comment, CommentDAO>, ICommentRepository
    {
        public CommentRepository(ECommerceContext ctx, IMapper<CommentDAO, Comment> mapper)
            : base(ctx, mapper) { }

        public async Task<IEnumerable<Comment>> GetAllByBaseItemIdAsync(Guid baseItemId)
        {
            var daos = await _dbSet.Where(c => c.BaseItemId == baseItemId).ToListAsync();
            return daos.Select(d => _mapper.Map(d));
        }
    }
}
