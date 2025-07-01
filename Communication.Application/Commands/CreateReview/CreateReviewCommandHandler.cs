using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Contracts.Queries;

namespace Communication.Application.Commands.CreateReview
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewRepository _reviewRepo;
        private readonly IMediator _mediator;

        public CreateReviewCommandHandler(
            IUnitOfWork unitOfWork,
            IReviewRepository reviewRepo,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _reviewRepo = reviewRepo;
            _mediator = mediator;
        }

        public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dto = request.DTO;
                var reviewerId = request.UserId;

                // 1. Validate input
                var validationResult = ValidateInput(dto);
                if (!validationResult.Success)
                    return validationResult;

                await _unitOfWork.BeginTransaction();

                try
                {
                    // 2. Get provider ID from the order
                    var providerQuery = new GetProviderIdByOrderIdQuery(dto.OrderId);
                    var providerResult = await _mediator.Send(providerQuery, cancellationToken);
                    
                    if (!providerResult.Success)
                        return Result<Guid>.Fail(
                            message: "لم يتم العثور على الطلب",
                            errorType: "OrderNotFound",
                            resultStatus: ResultStatus.NotFound);

                    var providerId = providerResult.Data.ProviderId;
                    if (providerId == Guid.Empty)
                        return Result<Guid>.Fail(
                            message: "لم يتم العثور على معرف المزود في الطلب",
                            errorType: "ProviderNotFound",
                            resultStatus: ResultStatus.NotFound);

                    // 3. Check if user already reviewed this order
                    var hasReviewed = await _reviewRepo.HasUserReviewedOrderAsync(reviewerId, dto.OrderId);
                    if (hasReviewed)
                        return Result<Guid>.Fail(
                            message: "لقد قمت بمراجعة هذا الطلب من قبل",
                            errorType: "DuplicateReview",
                            resultStatus: ResultStatus.ValidationError);

                    // 4. Create Review
                    var review = new Review
                    {
                        Id = Guid.NewGuid(),
                        ExperienceDescription = dto.ExperienceDescription,
                        OverallSatisfaction = dto.OverallSatisfaction,
                        ItemQuality = dto.ItemQuality,
                        Communication = dto.Communication,
                        Timeliness = dto.Timeliness,
                        ValueForMoney = dto.ValueForMoney,
                        NetPromoterScore = dto.NetPromoterScore,
                        WillUseAgain = dto.WillUseAgain,
                        ProviderId = providerId,
                        ReviewerId = reviewerId,
                        OrderId = dto.OrderId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _reviewRepo.AddAsync(review);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransaction();

                    return Result<Guid>.Ok(
                        data: review.Id,
                        message: "تم إضافة المراجعة بنجاح",
                        resultStatus: ResultStatus.Success);
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Result<Guid>.Fail(
                    message: "حدث خطأ أثناء إضافة المراجعة",
                    errorType: "CreateReviewFailed",
                    resultStatus: ResultStatus.InternalServerError,
                    exception: ex);
            }
        }

        private static Result<Guid> ValidateInput(CreateReviewDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ExperienceDescription))
                return Result<Guid>.Fail(
                    message: "وصف التجربة مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            
            if (dto.OverallSatisfaction < 1 || dto.OverallSatisfaction > 5)
                return Result<Guid>.Fail(
                    message: "التقييم العام يجب أن يكون بين 1 و 5",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            
            if (dto.ItemQuality < 1 || dto.ItemQuality > 5)
                return Result<Guid>.Fail(
                    message: "تقييم جودة المنتج يجب أن يكون بين 1 و 5",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            
            if (dto.Communication < 1 || dto.Communication > 5)
                return Result<Guid>.Fail(
                    message: "تقييم التواصل يجب أن يكون بين 1 و 5",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            
            if (dto.Timeliness < 1 || dto.Timeliness > 5)
                return Result<Guid>.Fail(
                    message: "تقييم الوقت يجب أن يكون بين 1 و 5",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            
            if (dto.NetPromoterScore < 0 || dto.NetPromoterScore > 10)
                return Result<Guid>.Fail(
                    message: "درجة التوصية يجب أن تكون بين 0 و 10",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);

            return Result<Guid>.Ok(
                data: Guid.Empty,
                message: null,
                resultStatus: ResultStatus.Success);
        }
    }
} 