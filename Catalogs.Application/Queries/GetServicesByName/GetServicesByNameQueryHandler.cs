using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Pagination;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalogs.Application.Queries.GetServicesByName;

public class GetServicesByNameQueryHandler : IRequestHandler<GetServicesByNameQuery, Result<PaginatedResult<ServiceDTO>>>
{
    private readonly IServiceRepository _repository;
    private readonly ILogger<GetServicesByNameQueryHandler> _logger;

    public GetServicesByNameQueryHandler(
        IServiceRepository repository,
        ILogger<GetServicesByNameQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetServicesByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "لا يمكن أن يكون اسم الخدمة فارغاً",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageNumber < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "يجب أن يكون رقم الصفحة أكبر من أو يساوي 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageSize < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "يجب أن يكون حجم الصفحة أكبر من أو يساوي 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var paginatedServices = await _repository.GetServicesByNameAsync(request.Name, request.Parameters.PageNumber, request.Parameters.PageSize);
            
            if (!paginatedServices.Data.Any())
            {
                return Result<PaginatedResult<ServiceDTO>>.Ok(
                    data: PaginatedResult<ServiceDTO>.Create(
                        data: new List<ServiceDTO>(),
                        pageNumber: request.Parameters.PageNumber,
                        pageSize: request.Parameters.PageSize,
                        totalCount: 0),
                    message: $"لم يتم العثور على خدمات تطابق الاسم '{request.Name}'",
                    resultStatus: ResultStatus.Success);
            }
            
            var dtos = paginatedServices.Data.Select(s => new ServiceDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                CategoryId = s.CategoryId,
                IsAvailable = s.IsAvailable,
                UserId = s.UserId
            }).ToList();

            var paginated = PaginatedResult<ServiceDTO>.Create(
                data: dtos,
                pageNumber: request.Parameters.PageNumber,
                pageSize: request.Parameters.PageSize,
                totalCount: paginatedServices.TotalCount);

            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginated,
                message: $"تم العثور على {dtos.Count} خدمة تطابق الاسم '{request.Name}'",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "خطأ في قاعدة البيانات أثناء البحث عن الخدمات باسم '{ServiceName}'", request.Name);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "فشل البحث عن الخدمات بسبب خطأ في قاعدة البيانات. يرجى المحاولة مرة أخرى لاحقاً",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ غير متوقع أثناء البحث عن الخدمات باسم '{ServiceName}': {Message}", request.Name, ex.Message);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "حدث خطأ غير متوقع أثناء البحث عن الخدمات. يرجى المحاولة مرة أخرى لاحقاً",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 