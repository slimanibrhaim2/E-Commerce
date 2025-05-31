using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System;
using System.Collections.Generic;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllConversations;

public record GetAllConversationsQuery(Guid UserId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ConversationDTO>>>; 