using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;
using Shared.Contracts.Queries;

namespace Communication.Application.Commands.UpdateComment;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Result<bool>>
{
    private readonly ICommentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateCommentCommandHandler(ICommentRepository repository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                return Result<bool>.Fail(
                    message: "التعليق غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }
            if (request.Comment.BaseContentId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "معرف المحتوى الأساسي مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.Comment.ItemId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "معرف العنصر مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            // Try to get BaseItemId from ProductId first
            var productQuery = new GetBaseItemIdByProductIdQuery(request.Comment.ItemId);
            var productResult = await _mediator.Send(productQuery, cancellationToken);
            
            Guid baseItemId;
            if (productResult.Success)
            {
                baseItemId = productResult.Data;
            }
            else
            {
                // If not a product, try to get BaseItemId from ServiceId
                var serviceQuery = new GetBaseItemIdByServiceIdQuery(request.Comment.ItemId);
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

            entity.BaseContentId = request.Comment.BaseContentId;
            entity.BaseItemId = baseItemId;
            entity.UpdatedAt = DateTime.UtcNow;
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(
                data: true,
                message: "تم تحديث التعليق بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"فشل في تحديث التعليق: {ex.Message}",
                errorType: "UpdateCommentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 