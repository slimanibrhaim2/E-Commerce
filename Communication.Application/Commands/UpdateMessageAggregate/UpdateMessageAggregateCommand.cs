using MediatR;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.UpdateMessageAggregate;

public record UpdateMessageAggregateCommand(Guid Id, AddMessageAggregateDTO DTO) : IRequest<Core.Result.Result<Guid>>; 