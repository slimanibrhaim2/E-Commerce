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
            ExperienceDescription = source.ExperienceDescription,
            OverallSatisfaction = source.OverallSatisfaction,
            ItemQuality = source.ItemQuality,
            Communication = source.Communication,
            Timeliness = source.Timeliness,
            ValueForMoney = source.ValueForMoney,
            NetPromoterScore = source.NetPromoterScore,
            WillUseAgain = source.WillUseAgain,
            Rating = source.Rating,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt,
            ProviderId = source.ProviderId,
            ReviewerId = source.ReviewerId,
            OrderId = source.OrderId
        };
    }

    public ReviewDAO MapBack(Review target)
    {
        if (target == null) return null;
        return new ReviewDAO
        {
            Id = target.Id,
            ExperienceDescription = target.ExperienceDescription,
            OverallSatisfaction = target.OverallSatisfaction,
            ItemQuality = target.ItemQuality,
            Communication = target.Communication,
            Timeliness = target.Timeliness,
            ValueForMoney = target.ValueForMoney,
            NetPromoterScore = target.NetPromoterScore,
            WillUseAgain = target.WillUseAgain,
            Rating = target.Rating,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt,
            ProviderId = target.ProviderId,
            ReviewerId = target.ReviewerId,
            OrderId = target.OrderId
        };
    }
} 