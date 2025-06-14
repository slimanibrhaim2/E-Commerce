using MediatR;
using System;
using Core.Result;

namespace Users.Application.Commands.DeleteFollower
{
    public record DeleteFollowerCommand(Guid FollowerId, Guid FollowingId) : IRequest<Result>;
}