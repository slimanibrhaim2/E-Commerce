using Core.Interfaces;
using Catalogs.Domain.Entities;
using System.Threading.Tasks;

namespace Catalogs.Domain.Repositories;

public interface IMediaRepository : IRepository<Media>
{
    Task<IEnumerable<Media>> GetMediaByItemIdAsync(Guid itemId);
    Task<bool> UpdateMediaAsync(Guid mediaId, string url, Guid typeId);
    Task<bool> DeleteMediaAsync(Guid mediaId);
    Task<Guid?> AddMediaAsync(Guid itemId, string url, Guid typeId);
} 