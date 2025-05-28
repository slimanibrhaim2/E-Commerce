using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.UpdateBaseContent;

public class UpdateBaseContentCommandHandler : IRequestHandler<UpdateBaseContentCommand, Result<bool>>
{
    private readonly IBaseContentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBaseContentCommandHandler(IBaseContentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateBaseContentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.BaseContent.UserId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "UserId is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (string.IsNullOrWhiteSpace(request.BaseContent.Title))
            {
                return Result<bool>.Fail(
                    message: "Title is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (string.IsNullOrWhiteSpace(request.BaseContent.Description))
            {
                return Result<bool>.Fail(
                    message: "Description is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<bool>.Fail(
                    message: "Base content not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            entity.UserId = request.BaseContent.UserId;
            entity.Title = request.BaseContent.Title;
            entity.Description = request.BaseContent.Description;
            entity.UpdatedAt = DateTime.UtcNow;
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(
                data: true,
                message: "Base content updated successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to update base content: {ex.Message}",
                errorType: "UpdateBaseContentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 