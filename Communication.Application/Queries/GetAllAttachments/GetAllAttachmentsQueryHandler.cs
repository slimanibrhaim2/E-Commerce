using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllAttachments;

public class GetAllAttachmentsQueryHandler : IRequestHandler<GetAllAttachmentsQuery, Result<PaginatedResult<AttachmentDTO>>>
{
    private readonly IAttachmentRepository _repository;

    public GetAllAttachmentsQueryHandler(IAttachmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PaginatedResult<AttachmentDTO>>> Handle(GetAllAttachmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            var totalCount = entities.Count();
            var data = entities
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .Select(e => new AttachmentDTO
                {
                    Id = e.Id,
                    BaseContentId = e.BaseContentId,
                    AttachmentUrl = e.AttachmentUrl,
                    AttachmentTypeId = e.AttachmentTypeId,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    DeletedAt = e.DeletedAt
                })
                .ToList();
            var paginated = Core.Pagination.PaginatedResult<AttachmentDTO>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<AttachmentDTO>>.Ok(
                paginated,
                message: "Attachments retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<AttachmentDTO>>.Fail(
                message: $"Failed to get attachments: {ex.Message}",
                errorType: "GetAllAttachmentsFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 