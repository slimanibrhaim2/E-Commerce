namespace Communication.Application.DTOs;
using System;

public class ReviewDTO
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
    public int? Rating { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // References
    public Guid ProviderId { get; set; }
    public Guid ReviewerId { get; set; }
    public Guid OrderId { get; set; }
} 