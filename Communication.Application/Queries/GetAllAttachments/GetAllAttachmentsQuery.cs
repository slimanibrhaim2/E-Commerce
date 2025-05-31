using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllAttachments;

public record GetAllAttachmentsQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<AttachmentDTO>>>; 