using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllConversationMembers;

public record GetAllConversationMembersQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ConversationMemberDTO>>>; 