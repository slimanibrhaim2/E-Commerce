using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;

namespace Catalogs.Application.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
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
            var category = await _categoryRepository.GetCategoryByIdAsync(request.Id);
            if (category == null)
            {
                return Result<bool>.Fail(
                    message: "الفئة غير موجودة",
                    errorType: "CategoryNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            var result = await _categoryRepository.DeleteCategoryAsync(request.Id);
            return Result<bool>.Ok(
                data: result,
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