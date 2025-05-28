using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Queries.GetConversationById;

public record GetConversationByIdQuery(Guid Id) : IRequest<Result<ConversationDTO>>; 