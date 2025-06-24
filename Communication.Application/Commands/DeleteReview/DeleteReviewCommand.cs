using MediatR;
using Core.Result;

namespace Communication.Application.Commands.DeleteReview
{
    public record DeleteReviewCommand(Guid ReviewId, Guid UserId) : IRequest<Result<bool>>;
} 