using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Catalogs.Application.Queries.GetAllBaseItemsByUserId;

public class GetAllBaseItemsByUserIdQueryHandler : IRequestHandler<GetAllBaseItemsByUserIdQuery, Result<List<BaseItemDTO>>>
{
    private readonly IBaseItemRepository _repository;

    public GetAllBaseItemsByUserIdQueryHandler(IBaseItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<BaseItemDTO>>> Handle(GetAllBaseItemsByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await _repository.GetAllByUserIdAsync(request.UserId);
            var dtos = entities.Select(entity => new BaseItemDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                IsAvailable = entity.IsAvailable,
                CategoryId = entity.CategoryId,
                UserId = entity.UserId
            }).ToList();
            return Result<List<BaseItemDTO>>.Ok(
                data: dtos,
                message: "Base items retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<List<BaseItemDTO>>.Fail(
                message: $"Failed to get base items: {ex.Message}",
                errorType: "GetAllBaseItemsByUserIdFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 