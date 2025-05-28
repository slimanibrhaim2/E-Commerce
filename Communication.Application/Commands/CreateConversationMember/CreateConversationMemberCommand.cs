using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.CreateConversationMember;

public record CreateConversationMemberCommand(CreateConversationMemberDTO ConversationMember) : IRequest<Result<Guid>>; 