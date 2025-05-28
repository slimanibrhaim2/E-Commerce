using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.CreateBaseContent;

public record CreateBaseContentCommand(CreateBaseContentDTO BaseContent) : IRequest<Result<Guid>>; 