using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.UpdateMessage;

public record UpdateMessageCommand(Guid Id, CreateMessageDTO Message) : IRequest<Result<bool>>; 