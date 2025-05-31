using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllAttachmentTypes;

public record GetAllAttachmentTypesQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<AttachmentTypeDTO>>>; 