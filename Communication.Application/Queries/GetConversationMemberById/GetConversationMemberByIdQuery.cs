using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Queries.GetConversationMemberById;

public record GetConversationMemberByIdQuery(Guid Id) : IRequest<Result<ConversationMemberDTO>>; 