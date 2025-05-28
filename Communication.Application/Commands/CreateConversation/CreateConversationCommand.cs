using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.CreateConversation;

public record CreateConversationCommand(CreateConversationDTO Conversation) : IRequest<Result<Guid>>; 