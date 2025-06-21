using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;
using Shared.Contracts.Queries;

namespace Communication.Application.Commands.CreateComment;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<Guid>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CreateCommentCommandHandler(ICommentRepository commentRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Comment.BaseContentId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "معرف المحتوى الأساسي مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }
            if (request.Comment.ItemId == Guid.Empty)
            {
                return Result<Guid>.Fail(
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
                    return Result<Guid>.Fail(
                        message: "العنصر غير موجود",
                        errorType: "ItemNotFound",
                        resultStatus: ResultStatus.NotFound);
                }
            }

            var comment = new Comment
            {
                BaseContentId = request.Comment.BaseContentId,
                BaseItemId = baseItemId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Ok(
                data: comment.Id,
                message: "تم إنشاء التعليق بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
                message: $"فشل في إنشاء التعليق: {ex.Message}",
                errorType: "CreateCommentFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 