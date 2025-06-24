using MediatR;
using Core.Result;
using System;

namespace Communication.Application.Queries.CheckIfReviewed;

public record CheckIfReviewedQuery(Guid ItemId, Guid UserId) : IRequest<Result<bool>>; 