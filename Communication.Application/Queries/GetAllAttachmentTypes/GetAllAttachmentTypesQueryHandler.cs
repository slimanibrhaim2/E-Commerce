using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using AutoMapper;

namespace Communication.Application.Queries.GetAllAttachmentTypes;

public class GetAllAttachmentTypesQueryHandler : IRequestHandler<GetAllAttachmentTypesQuery, Result<List<AttachmentTypeDTO>>>
{
    private readonly IAttachmentTypeRepository _repository;
    private readonly IMapper _mapper;

    public GetAllAttachmentTypesQueryHandler(IAttachmentTypeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<List<AttachmentTypeDTO>>> Handle(GetAllAttachmentTypesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            var dtos = entities.Select(e => new AttachmentTypeDTO
            {
                Id = e.Id,
                Name = e.Name,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                DeletedAt = e.DeletedAt
            }).ToList();
            return Result<List<AttachmentTypeDTO>>.Ok(
                data: dtos,
                message: "Attachment types retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<List<AttachmentTypeDTO>>.Fail(
                message: $"Failed to get attachment types: {ex.Message}",
                errorType: "GetAllAttachmentTypesFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 