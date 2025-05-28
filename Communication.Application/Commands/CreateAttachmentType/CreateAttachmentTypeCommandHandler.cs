using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;

namespace Communication.Application.Commands.CreateAttachmentType;

public class CreateAttachmentTypeCommandHandler : IRequestHandler<CreateAttachmentTypeCommand, Result<Guid>>
{
    private readonly IAttachmentTypeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAttachmentTypeCommandHandler(IAttachmentTypeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateAttachmentTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.AttachmentType.Name))
            {
                return Result<Guid>.Fail(
                    message: "Name is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var entity = new AttachmentType
            {
                Name = request.AttachmentType.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(
                data: entity.Id,
                message: "Attachment type created successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
                message: $"Failed to create attachment type: {ex.Message}",
                errorType: "CreateAttachmentTypeFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 