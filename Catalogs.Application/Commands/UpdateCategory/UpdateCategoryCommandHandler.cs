using Catalogs.Domain.Repositories;
using Core.Result;
using MediatR;
using Catalogs.Domain.Entities;
using Catalogs.Application.DTOs;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<bool>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository, 
        IUnitOfWork unitOfWork,
        IFileService fileService,
        ILogger<UpdateCategoryCommandHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Category.Name))
        {
            return Result<bool>.Fail(
                message: "اسم التصنيف مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            // Get the category using Category repository
            var category = await _categoryRepository.GetCategoryByIdAsync(request.Id);
            if (category == null)
            {
                return Result<bool>.Fail(
                    message: "لم يتم العثور على التصنيف",
                    errorType: "CategoryNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Handle old image deletion if a new image is provided
            var oldImageUrl = category.ImageUrl;
            var newImageUrl = request.Category.ImageUrl;
            
            if (!string.IsNullOrEmpty(oldImageUrl) && 
                !string.IsNullOrEmpty(newImageUrl) && 
                oldImageUrl != newImageUrl)
            {
                // Delete old image file
                var relativePath = ExtractRelativePathFromUrl(oldImageUrl);
                var deleteResult = _fileService.DeleteFile(relativePath);
                if (!deleteResult.Success)
                {
                    _logger.LogWarning("Failed to delete old category image: {Error}. URL: {ImageUrl}, RelativePath: {RelativePath}", 
                        deleteResult.Message, oldImageUrl, relativePath);
                }
                else
                {
                    _logger.LogInformation("Successfully deleted old category image: {RelativePath}", relativePath);
                }
            }

            // Update category properties
            category.Name = request.Category.Name;
            category.Description = request.Category.Description;
            category.ImageUrl = newImageUrl;
            category.IsActive = request.Category.IsActive;
            category.ParentCategoryId = request.Category.ParentId;
            
            _logger.LogInformation("Updating category {CategoryId}: OldImageUrl={OldImageUrl}, NewImageUrl={NewImageUrl}", 
                request.Id, oldImageUrl, newImageUrl);

            // Update the category using Category repository
            var result = await _categoryRepository.UpdateCategoryAsync(request.Id, category);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(
                data: result,
                message: "تم تحديث التصنيف بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"فشل في تحديث التصنيف: {ex.Message}",
                errorType: "UpdateCategoryFailed",
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