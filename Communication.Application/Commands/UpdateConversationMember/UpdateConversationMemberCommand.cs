using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.UpdateConversationMember;

public record UpdateConversationMemberCommand(Guid Id, CreateConversationMemberDTO ConversationMember) : IRequest<Result<bool>>; 