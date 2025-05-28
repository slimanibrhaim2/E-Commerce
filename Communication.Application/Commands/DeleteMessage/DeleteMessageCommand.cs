using MediatR;
using Core.Result;

namespace Communication.Application.Commands.DeleteMessage;

public record DeleteMessageCommand(Guid Id) : IRequest<Result<bool>>; 