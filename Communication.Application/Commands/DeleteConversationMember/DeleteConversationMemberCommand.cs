using MediatR;
using Core.Result;

namespace Communication.Application.Commands.DeleteConversationMember;

public record DeleteConversationMemberCommand(Guid Id) : IRequest<Result<bool>>; 