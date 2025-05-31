using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using AutoMapper;
using System.Linq;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllAttachmentTypes;

public class GetAllAttachmentTypesQueryHandler : IRequestHandler<GetAllAttachmentTypesQuery, Result<PaginatedResult<AttachmentTypeDTO>>>
{
    private readonly IAttachmentTypeRepository _repository;
    private readonly IMapper _mapper;

    public GetAllAttachmentTypesQueryHandler(IAttachmentTypeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedResult<AttachmentTypeDTO>>> Handle(GetAllAttachmentTypesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            var totalCount = entities.Count();
            var data = entities
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .Select(e => new AttachmentTypeDTO
                {
                    Id = e.Id,
                    Name = e.Name,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    DeletedAt = e.DeletedAt
                })
                .ToList();
            var paginated = Core.Pagination.PaginatedResult<AttachmentTypeDTO>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<AttachmentTypeDTO>>.Ok(
                paginated,
                message: "Attachment types retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<AttachmentTypeDTO>>.Fail(
                message: $"Failed to get attachment types: {ex.Message}",
                errorType: "GetAllAttachmentTypesFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 