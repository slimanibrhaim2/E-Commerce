using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;

    public DeleteCategoryCommandHandler(
        ICategoryRepository categoryRepository, 
        IUnitOfWork unitOfWork,
        IFileService fileService,
        ILogger<DeleteCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _logger = logger;
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

            // Delete associated image file if it exists
            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                var relativePath = ExtractRelativePathFromUrl(category.ImageUrl);
                var deleteResult = _fileService.DeleteFile(relativePath);
                if (!deleteResult.Success)
                {
                    _logger.LogWarning("Failed to delete category image during deletion: {Error}. URL: {ImageUrl}, RelativePath: {RelativePath}", 
                        deleteResult.Message, category.ImageUrl, relativePath);
                }
                else
                {
                    _logger.LogInformation("Successfully deleted category image: {RelativePath}", relativePath);
                }
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

    /// <summary>
    /// Extracts the relative path from a full URL for file deletion
    /// </summary>
    /// <param name="fullUrl">The full URL (e.g., http://localhost:5000/media/categories/2024/01/15/guid.jpg)</param>
    /// <returns>The relative path (e.g., media/categories/2024/01/15/guid.jpg)</returns>
    private string ExtractRelativePathFromUrl(string fullUrl)
    {
        if (string.IsNullOrEmpty(fullUrl))
            return string.Empty;

        try
        {
            // If it's already a relative path (doesn't start with http), return as is
            if (!fullUrl.StartsWith("http://") && !fullUrl.StartsWith("https://"))
            {
                return fullUrl.TrimStart('/');
            }

            // Parse the URL and extract the path part
            var uri = new Uri(fullUrl);
            var relativePath = uri.AbsolutePath.TrimStart('/');
            
            return relativePath;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract relative path from URL: {FullUrl}", fullUrl);
            // Fallback: try to extract everything after the domain
            var pathStart = fullUrl.IndexOf('/', 8); // Start after "http://"
            if (pathStart > 0)
            {
                return fullUrl.Substring(pathStart + 1); // Remove the leading slash
            }
            return fullUrl; // Return original if all else fails
        }
    }
} 