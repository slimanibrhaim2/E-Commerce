using Infrastructure.Common;
using Infrastructure.Models;
using Users.Domain.Entities;

namespace Users.Infrastructure.Mapping.Mappers;

public class UserRatingMapper : BaseMapper<UserRatingDAO, UserRating>
{
    public override UserRating Map(UserRatingDAO source)
    {
        return SafeMap(source, s => new UserRating
        {
            Id = s.Id,
            UserId = s.UserId,
            NumOfReviews = s.NumOfReviews,
            Rating = s.Rating,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt,
            DeletedAt = s.DeletedAt
        });
    }

    public override UserRatingDAO MapBack(UserRating target)
    {
        return SafeMapBack(target, t => new UserRatingDAO
        {
            Id = t.Id,
            UserId = t.UserId,
            NumOfReviews = t.NumOfReviews,
            Rating = t.Rating,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            DeletedAt = t.DeletedAt
        });
    }
} 