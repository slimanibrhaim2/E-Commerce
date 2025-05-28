using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.UpdateConversation;

public record UpdateConversationCommand(Guid Id, CreateConversationDTO Conversation) : IRequest<Result<bool>>; 