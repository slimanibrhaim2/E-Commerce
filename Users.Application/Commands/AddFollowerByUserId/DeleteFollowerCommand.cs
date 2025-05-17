using MediatR;
using System;
using Core.Result;

namespace Users.Application.Commands.AddFollowerByUserId
{
    public record DeleteFollowerCommand(Guid FollowerId) : IRequest<Result>;
} 