using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.CreateMessage;

public record CreateMessageCommand(CreateMessageDTO Message) : IRequest<Result<Guid>>; 