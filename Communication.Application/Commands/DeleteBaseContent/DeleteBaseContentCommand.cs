using MediatR;
using Core.Result;

namespace Communication.Application.Commands.DeleteBaseContent;

public record DeleteBaseContentCommand(Guid Id) : IRequest<Result<bool>>; 