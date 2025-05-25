using Infrastructure.Common;
using Infrastructure.Models;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Users.Infrastructure.Mapping.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Users.Infrastructure.Repositories
{
    public class NotificationRepository : BaseRepository<Notification, NotificationDAO>, INotificationRepository
    {
        private readonly ECommerceContext _ctx;
        private readonly IMapper<NotificationDAO, Notification> _notificationMapper;

        public NotificationRepository(ECommerceContext ctx, IMapper<NotificationDAO, Notification> notificationMapper)
            : base(ctx, notificationMapper)
        {
            _ctx = ctx;
            _notificationMapper = notificationMapper;
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
        {
            var daos = await _dbSet.Where(n => n.UserId == userId && n.DeletedAt == null).ToListAsync();
            return daos.Select(n => _notificationMapper.Map(n));
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            var dao = await _dbSet.FirstOrDefaultAsync(n => n.Id == id && n.DeletedAt == null);
            return dao != null ? _notificationMapper.Map(dao) : null;
        }

        public async Task AddAsync(Notification entity)
        {
            var dao = _notificationMapper.MapBack(entity);
            await _ctx.Set<NotificationDAO>().AddAsync(dao);
        }

        public void Update(Notification entity)
        {
            var dao = _notificationMapper.MapBack(entity);
            _ctx.Set<NotificationDAO>().Update(dao);
        }

        public void Remove(Notification entity)
        {
            var dao = _notificationMapper.MapBack(entity);
            _ctx.Set<NotificationDAO>().Remove(dao);
        }
    }
}
