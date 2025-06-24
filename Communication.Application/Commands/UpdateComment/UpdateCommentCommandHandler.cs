using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;
using Shared.Contracts.Queries;
using System.Linq;

namespace Communication.Application.Commands.UpdateComment;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Result<bool>>
{
    private readonly ICommentRepository _repository;
    private readonly IBaseContentRepository _baseContentRepository;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateCommentCommandHandler(
        ICommentRepository repository, 
        IBaseContentRepository baseContentRepository,
        IAttachmentRepository attachmentRepository,
        IUnitOfWork unitOfWork, 
        IMediator mediator)
    {
        _repository = repository;
        _baseContentRepository = baseContentRepository;
        _attachmentRepository = attachmentRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransaction();
        try
        {
            var dto = request.Comment;
            var userId = request.UserId;
            
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<bool>.Fail(
                    message: "التعليق غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                return Result<bool>.Fail(
                    message: "محتوى التعليق مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (dto.ItemId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "معرف العنصر مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            // Resolve BaseItemId from ItemId
            var productQuery = new GetBaseItemIdByProductIdQuery(dto.ItemId);
            var productResult = await _mediator.Send(productQuery, cancellationToken);
            
            Guid baseItemId;
            if (productResult.Success)
            {
                baseItemId = productResult.Data;
            }
            else
            {
                var serviceQuery = new GetBaseItemIdByServiceIdQuery(dto.ItemId);
                var serviceResult = await _mediator.Send(serviceQuery, cancellationToken);
                
                if (serviceResult.Success)
                {
                    baseItemId = serviceResult.Data;
                }
                else
                {
                    return Result<bool>.Fail(
                        message: "العنصر غير موجود",
                        errorType: "ItemNotFound",
                        resultStatus: ResultStatus.NotFound);
                }
            }

            // Update BaseContent
            var baseContent = await _baseContentRepository.GetByIdAsync(entity.BaseContentId);
            if (baseContent != null)
            {
                baseContent.Title = dto.Content;
                baseContent.Description = dto.Content;
                baseContent.UpdatedAt = DateTime.UtcNow;
                _baseContentRepository.Update(baseContent);
            }

            // Update Comment
            entity.Content = dto.Content;
            entity.BaseItemId = baseItemId;
            entity.UpdatedAt = DateTime.UtcNow;
            _repository.Update(entity);

            // Update Attachments - Remove existing and add new ones
            if (baseContent != null)
            {
                // Get existing attachments
                var existingAttachments = await _attachmentRepository.FindAsync(a => a.BaseContentId == baseContent.Id);
                
                // Remove existing attachments
                foreach (var attachment in existingAttachments)
                {
                    _attachmentRepository.Remove(attachment);
                }

                // Add new attachments
                foreach (var attachmentDto in dto.Attachments ?? Enumerable.Empty<AttachmentDTO>())
                {
                    var attachment = new Attachment
                    {
                        Id = Guid.NewGuid(),
                        BaseContentId = baseContent.Id,
                        AttachmentUrl = attachmentDto.AttachmentUrl,
                        AttachmentTypeId = attachmentDto.AttachmentTypeId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _attachmentRepository.AddAsync(attachment);
                }
            }
            
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransaction();
            
            return Result<bool>.Ok(
                data: true,
                message: "تم تحديث التعليق بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransaction();
            return Result<bool>.Fail(
                message: $"فشل في تحديث التعليق: {ex.Message}",
                errorType: "UpdateCommentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 