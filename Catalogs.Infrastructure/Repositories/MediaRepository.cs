using Microsoft.EntityFrameworkCore;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;

namespace Catalogs.Infrastructure.Repositories;

public class MediaRepository : BaseRepository<Media, MediaDAO>, IMediaRepository
{
    public MediaRepository(ECommerceContext context, IMapper<MediaDAO, Media> mapper)
        : base(context, mapper)
    {
    }

    public async Task<Media?> GetByIdAsync(Guid id)
    {
        var mediaDao = await _ctx.Media.FirstOrDefaultAsync(m => m.Id == id && m.DeletedAt == null);
        return mediaDao == null ? null : _mapper.Map(mediaDao);
    }

    public async Task<IEnumerable<Media>> GetMediaByItemIdAsync(Guid itemId)
    {
        var media = await _ctx.Media.Where(m => m.BaseItemId == itemId && m.DeletedAt == null).ToListAsync();
        return media.Select(m => _mapper.Map(m));
    }

    public async Task<bool> UpdateMediaAsync(Guid mediaId, string url, Guid mediaTypeId)
    {
        var mediaDao = await _ctx.Media.FirstOrDefaultAsync(m => m.Id == mediaId && m.DeletedAt == null);
        if (mediaDao == null) return false;

        mediaDao.MediaUrl = url;
        mediaDao.MediaTypeId = mediaTypeId;
        mediaDao.UpdatedAt = DateTime.UtcNow;
        // Update other properties as needed

        await _ctx.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteMediaAsync(Guid mediaId)
    {
        var mediaDao = await _ctx.Media.FirstOrDefaultAsync(m => m.Id == mediaId && m.DeletedAt == null);
        if (mediaDao == null) return false;

        mediaDao.DeletedAt = DateTime.UtcNow;
        await _ctx.SaveChangesAsync();
        return true;
    }

    public async Task<Guid?> AddMediaAsync(Guid itemId, string url, Guid mediaTypeId)
    {
        // Check if itemId exists in BaseItems (products/services)
        var baseItem = await _ctx.BaseItems.FirstOrDefaultAsync(b => b.Id == itemId && b.DeletedAt == null);
        if (baseItem == null)
            return null;

        var mediaDao = new MediaDAO
        {
            Id = Guid.NewGuid(),
            BaseItemId = itemId,
            MediaUrl = url,
            MediaTypeId = mediaTypeId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _ctx.Media.Add(mediaDao);
        await _ctx.SaveChangesAsync();
        return mediaDao.Id;
    }
} 