using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.UpdateReview
{
    public record UpdateReviewCommand(Guid ReviewId, UpdateReviewDTO DTO, Guid UserId) : IRequest<Result<bool>>;
} 