using Infrastructure.Common;
using Infrastructure.Models;
using Communication.Domain.Entities;

namespace Communication.Infrastructure.Mapping.Mappers;

public class ReviewMapper : IMapper<ReviewDAO, Review>
{
    public Review Map(ReviewDAO source)
    {
        if (source == null) return null;
        return new Review
        {
            Id = source.Id,
            UserId = source.UserId,
            BaseItemId = source.BaseItemId,
            OrderId = source.OrderId,
            Title = source.Title,
            Content = source.Content,
            IsVerifiedPurchase = source.IsVerifiedPurchase,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt
        };
    }

    public ReviewDAO MapBack(Review target)
    {
        if (target == null) return null;
        return new ReviewDAO
        {
            Id = target.Id,
            UserId = target.UserId,
            BaseItemId = target.BaseItemId,
            OrderId = target.OrderId,
            Title = target.Title,
            Content = target.Content,
            IsVerifiedPurchase = target.IsVerifiedPurchase,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt
        };
    }
} 