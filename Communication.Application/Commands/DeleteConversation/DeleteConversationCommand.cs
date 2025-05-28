using MediatR;
using Core.Result;

namespace Communication.Application.Commands.DeleteConversation;

public record DeleteConversationCommand(Guid Id) : IRequest<Result<bool>>; 