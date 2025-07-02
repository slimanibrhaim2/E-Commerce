using System;
using MediatR;
 
namespace Users.Application.Commands.CreateUserRating
{
    public record CreateUserRatingCommand(Guid UserId) : IRequest<bool>;
} 