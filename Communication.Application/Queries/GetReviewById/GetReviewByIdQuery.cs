using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System;

namespace Communication.Application.Queries.GetReviewById;

public record GetReviewByIdQuery(Guid ReviewId) : IRequest<Result<ReviewDTO>>; 