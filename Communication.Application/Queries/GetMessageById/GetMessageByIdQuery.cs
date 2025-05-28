using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Queries.GetMessageById;

public record GetMessageByIdQuery(Guid Id) : IRequest<Result<MessageDTO>>; 