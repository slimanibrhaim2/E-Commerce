// Users.Application/Commands/AddFollowerByUserId/AddFollowerByUserIdCommand.cs
using MediatR;
using System;
using Core.Result;

namespace Users.Application.Command.AddFollowerByUserId;

public record AddFollowerByUserIdCommand(
    Guid FollowingId,   // the user being followed
    Guid FollowerId     // the user who follows
) : IRequest<Result<Guid>>;
