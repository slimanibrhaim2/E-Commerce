using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.CreateReview
{
    public record CreateReviewCommand(CreateReviewDTO DTO, Guid UserId) : IRequest<Result<Guid>>;
} 