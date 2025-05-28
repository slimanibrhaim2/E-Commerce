using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Communication.Application.Queries.GetAllAttachments;

public class GetAllAttachmentsQueryHandler : IRequestHandler<GetAllAttachmentsQuery, Result<List<AttachmentDTO>>>
{
    private readonly IAttachmentRepository _repository;

    public GetAllAttachmentsQueryHandler(IAttachmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<AttachmentDTO>>> Handle(GetAllAttachmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            var dtos = entities.Select(e => new AttachmentDTO
            {
                Id = e.Id,
                BaseContentId = e.BaseContentId,
                AttachmentUrl = e.AttachmentUrl,
                AttachmentTypeId = e.AttachmentTypeId,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                DeletedAt = e.DeletedAt
            }).ToList();
            return Result<List<AttachmentDTO>>.Ok(
                data: dtos,
                message: "Attachments retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<List<AttachmentDTO>>.Fail(
                message: $"Failed to get attachments: {ex.Message}",
                errorType: "GetAllAttachmentsFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 