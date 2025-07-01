using MediatR;
using Core.Result;
using System;

namespace Communication.Application.Queries.CheckIfReviewed;

public record CheckIfReviewedQuery(Guid OrderId, Guid UserId) : IRequest<Result<bool>>; 