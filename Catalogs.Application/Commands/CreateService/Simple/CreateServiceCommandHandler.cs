using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Interfaces;

namespace Catalogs.Application.Commands.CreateService.Simple;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Result<Guid>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<CreateServiceCommandHandler> _logger;
    private readonly IBaseItemRepository _baseItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateServiceCommandHandler(IServiceRepository repo, ILogger<CreateServiceCommandHandler> logger, IBaseItemRepository baseItemRepository, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _logger = logger;
        _baseItemRepository = baseItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Service == null)
                return Result<Guid>.Fail(
                    message: "Service data is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);

            // Validation
            if (string.IsNullOrWhiteSpace(request.Service.Name))
            {
                return Result<Guid>.Fail(
                    message: "اسم الخدمة مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.Service.Price <= 0)
            {
                return Result<Guid>.Fail(
                    message: "سعر الخدمة يجب أن يكون أكبر من الصفر",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.Service.CategoryId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "معرف التصنيف مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            // 1. Create and persist BaseItem first
            var baseItem = new BaseItem
            {
                Id = Guid.NewGuid(),
                Name = request.Service.Name,
                Description = request.Service.Description,
                Price = request.Service.Price,
                CategoryId = request.Service.CategoryId,
                UserId = request.UserId,
                IsAvailable = request.Service.IsAvailable
            };
            await _baseItemRepository.AddAsync(baseItem);
            await _unitOfWork.SaveChangesAsync();

            // 2. Create service with BaseItemId as FK (if applicable)
            Service service = new Service
            {
                Id = Guid.NewGuid(),
                BaseItemId=baseItem.Id,
                Name = request.Service.Name,
                Description = request.Service.Description,
                Price = request.Service.Price,
                CategoryId = request.Service.CategoryId,
                ServiceType = request.Service.ServiceType,
                Duration = request.Service.Duration,
                IsAvailable = request.Service.IsAvailable,
                UserId = request.UserId,
                // Set the BaseItemId or link to the baseItem as needed
            };
            // If Service has a BaseItemId property, set it here. If not, ensure mapping is correct in the repo/mapper.
            // service.BaseItemId = baseItem.Id; // Uncomment if such property exists
            await _repo.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Ok(
                data: service.Id,
                message: "تم إنشاء الخدمة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating service");
            return Result<Guid>.Fail(
                message: "حدث خطأ أثناء إنشاء الخدمة",
                errorType: "CreateServiceFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 