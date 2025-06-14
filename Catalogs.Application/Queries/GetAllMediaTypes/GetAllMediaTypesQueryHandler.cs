using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Pagination;
using System.Data;

namespace Catalogs.Application.Queries.GetAllMediaTypes;

public class GetAllMediaTypesQueryHandler : IRequestHandler<GetAllMediaTypesQuery, Result<PaginatedResult<MediaTypeDTO>>>
{
    private readonly IMediaTypeRepository _repo;
    private readonly ILogger<GetAllMediaTypesQueryHandler> _logger;

    public GetAllMediaTypesQueryHandler(IMediaTypeRepository repo, ILogger<GetAllMediaTypesQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<MediaTypeDTO>>> Handle(GetAllMediaTypesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Parameters.PageNumber < 1)
            {
                return Result<PaginatedResult<MediaTypeDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageSize < 1)
            {
                return Result<PaginatedResult<MediaTypeDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var mediaTypes = await _repo.GetAllAsync();
            var mediaTypeList = mediaTypes.ToList();
            var totalCount = mediaTypeList.Count;

            if (!mediaTypeList.Any())
            {
                return Result<PaginatedResult<MediaTypeDTO>>.Ok(
                    data: PaginatedResult<MediaTypeDTO>.Create(
                        data: new List<MediaTypeDTO>(),
                        pageNumber: request.Parameters.PageNumber,
                        pageSize: request.Parameters.PageSize,
                        totalCount: 0),
                    message: "No media types found",
                    resultStatus: ResultStatus.Success);
            }

            var mediaTypeDtos = mediaTypeList.Select(mt => new MediaTypeDTO
            {
                Id = mt.Id,
                Name = mt.Name,
                CreatedAt = mt.CreatedAt,
                UpdatedAt = mt.UpdatedAt
            }).ToList();

            var paginatedMediaTypes = PaginatedResult<MediaTypeDTO>.Create(
                data: mediaTypeDtos,
                pageNumber: request.Parameters.PageNumber,
                pageSize: request.Parameters.PageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<MediaTypeDTO>>.Ok(
                data: paginatedMediaTypes,
                message: $"Successfully retrieved {mediaTypeDtos.Count} media types out of {totalCount} total media types",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving media types");
            return Result<PaginatedResult<MediaTypeDTO>>.Fail(
                message: "Failed to retrieve media types due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving media types: {Message}", ex.Message);
            return Result<PaginatedResult<MediaTypeDTO>>.Fail(
                message: "An unexpected error occurred while retrieving media types. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 