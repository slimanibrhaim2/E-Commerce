using Infrastructure.Common;
using Infrastructure.Models;
using Users.Domain.Entities;

namespace Users.Infrastructure.Mapping.Mappers;

public class FollowerMapper : BaseMapper<FollowerDAO, Follower>
{
    private readonly UserMapper _userMapper = new UserMapper();

    public override Follower Map(FollowerDAO source)
    {
        return SafeMap(source, s => new Follower
        {
            Id = s.Id,
            FollowerId = s.FollowerId,
            FollowingId = s.FollowingId,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            DeletedAt = s.DeletedAt,
            FollowerUser = s.Follower != null ? _userMapper.Map(s.Follower) : null,
            Following = s.Following != null ? _userMapper.Map(s.Following) : null
        });
    }

    public override FollowerDAO MapBack(Follower target)
    {
        return SafeMapBack(target, t => new FollowerDAO
        {
            Id = t.Id,
            FollowerId = t.FollowerId,
            FollowingId = t.FollowingId,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            DeletedAt = t.DeletedAt
        });
    }
}