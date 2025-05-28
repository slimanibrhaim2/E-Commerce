using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.CreateBaseContent;

public class CreateBaseContentCommandHandler : IRequestHandler<CreateBaseContentCommand, Result<Guid>>
{
    private readonly IBaseContentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBaseContentCommandHandler(IBaseContentRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateBaseContentCommand request, CancellationToken cancellationToken)
    {
        if (request.BaseContent.UserId == Guid.Empty)
        {
            return Result<Guid>.Fail(
                message: "UserId is required",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (string.IsNullOrWhiteSpace(request.BaseContent.Title))
        {
            return Result<Guid>.Fail(
                message: "Title is required",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (string.IsNullOrWhiteSpace(request.BaseContent.Description))
        {
            return Result<Guid>.Fail(
                message: "Description is required",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        try
        {
            var entity = new BaseContent
            {
                UserId = request.BaseContent.UserId,
                Title = request.BaseContent.Title,
                Description = request.BaseContent.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(
                data: entity.Id,
                message: "Base content created successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
                message: $"Failed to create base content: {ex.Message}",
                errorType: "CreateBaseContentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 