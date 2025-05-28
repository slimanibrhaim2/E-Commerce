using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;

namespace Communication.Application.Queries.GetAllConversationMembers;

public record GetAllConversationMembersQuery() : IRequest<Result<List<ConversationMemberDTO>>>; 