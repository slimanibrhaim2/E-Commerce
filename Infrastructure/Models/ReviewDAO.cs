using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class ReviewDAO
{
    public Guid Id { get; set; }

    public string ExperienceDescription { get; set; }

    public int OverallSatisfaction { get; set; }

    public int ItemQuality { get; set; }

    public int Communication { get; set; }

    public int Timeliness { get; set; }

    public string ValueForMoney { get; set; }

    public int NetPromoterScore { get; set; }

    public bool WillUseAgain { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid ProviderId { get; set; }

    public Guid ReviewerId { get; set; }

    public Guid OrderId { get; set; }

    public virtual UserDAO Reviewer { get; set; }

    public virtual UserDAO Provider { get; set; }

    public virtual OrderDAO Order { get; set; }
} 