using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllMessages;

public record GetAllMessagesQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<MessageDTO>>>; 