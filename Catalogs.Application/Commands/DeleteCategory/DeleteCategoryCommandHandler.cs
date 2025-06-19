using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Core.Interfaces;

namespace Catalogs.Application.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (request.Id == Guid.Empty)
        {
            return Result<bool>.Fail(
                message: "معرف الفئة مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                return Result<bool>.Fail(
                    message: "الفئة غير موجودة",
                    errorType: "CategoryNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Check if already deleted
            if (category.DeletedAt != null)
            {
                return Result<bool>.Fail(
                    message: "الفئة محذوفة بالفعل",
                    errorType: "AlreadyDeleted",
                    resultStatus: ResultStatus.ValidationError);
            }

            // Use the standard Remove method for soft delete
            _categoryRepository.Remove(category);
            await _unitOfWork.SaveChangesAsync();
            
            return Result<bool>.Ok(
                data: true,
                message: "تم حذف الفئة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"فشل في حذف الفئة: {ex.Message}",
                errorType: "DeleteCategoryFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 