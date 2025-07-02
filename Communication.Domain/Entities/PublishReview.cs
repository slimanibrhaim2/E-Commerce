using System;

namespace Communication.Domain.Entities
{
    public class PublishReview
    {
        public Guid Id { get; set; }

        // Review Content
        public string ExperienceDescription { get; set; }
        public int OverallSatisfaction { get; set; }
        public int ItemQuality { get; set; }
        public int Communication { get; set; }
        public int Timeliness { get; set; }
        public string ValueForMoney { get; set; }
        public int NetPromoterScore { get; set; }
        public bool WillUseAgain { get; set; }

        public Guid ProviderId { get; set; }
    }
} 