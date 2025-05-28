using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System;
using System.Collections.Generic;

namespace Communication.Application.Queries.GetAllConversations;

public record GetAllConversationsQuery(Guid UserId) : IRequest<Result<List<ConversationDTO>>>; 