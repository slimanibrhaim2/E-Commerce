using Infrastructure.Common;
using Infrastructure.Models;
using Users.Domain.Entities;

namespace Users.Infrastructure.Mapping;

public class FollowerMapper : BaseMapper<FollowerDAO, Follower>
{
    public override Follower Map(FollowerDAO source)
    {
        return SafeMap(source, s => new Follower
        {
            Id = s.Id,
            FollowerId = s.FollowerId,
            FollowingId = s.FollowingId
        });
    }

    public override FollowerDAO MapBack(Follower target)
    {
        return SafeMapBack(target, t => new FollowerDAO
        {
            Id = t.Id,
            FollowerId = t.FollowerId,
            FollowingId = t.FollowingId
        });
    }
} 