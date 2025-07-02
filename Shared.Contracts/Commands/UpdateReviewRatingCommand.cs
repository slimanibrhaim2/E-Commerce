using MediatR;
using Core.Result;
using System;

namespace Shared.Contracts.Commands
{
    public record UpdateReviewRatingCommand(
        Guid ReviewId,
        int Rating) : IRequest<Result<bool>>;
} 