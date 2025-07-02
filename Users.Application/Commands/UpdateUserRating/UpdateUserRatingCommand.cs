using System;
using MediatR;
using Core.Result;

namespace Users.Application.Commands.UpdateUserRating
{
    public record UpdateUserRatingCommand(
        Guid UserId,
        int Rating,
        Guid ReviewId) : IRequest<Result<bool>>;
} 