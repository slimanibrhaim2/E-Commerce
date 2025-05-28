using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.UpdateBaseContent;

public record UpdateBaseContentCommand(Guid Id, CreateBaseContentDTO BaseContent) : IRequest<Result<bool>>; 