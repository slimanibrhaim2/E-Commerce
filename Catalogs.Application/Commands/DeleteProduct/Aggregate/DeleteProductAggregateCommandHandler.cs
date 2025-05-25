using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Catalogs.Application.Commands.DeleteProduct.Aggregate;

public class DeleteProductAggregateCommandHandler : IRequestHandler<DeleteProductAggregateCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IFeatureRepository _featureRepository;
    private readonly IBaseItemRepository _baseItemRepository;

    public DeleteProductAggregateCommandHandler(
        IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        IMediaRepository mediaRepository,
        IFeatureRepository featureRepository,
        IBaseItemRepository baseItemRepository)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _mediaRepository = mediaRepository;
        _featureRepository = featureRepository;
        _baseItemRepository = baseItemRepository;
    }

    public async Task<Result<bool>> Handle(DeleteProductAggregateCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransaction();
        try
        {
            if (request.Id == Guid.Empty)
                return Result<bool>.Fail("معرف المنتج مطلوب", "ValidationError", ResultStatus.ValidationError);

            var product = await _productRepository.GetById(request.Id);
            if (product == null)
                return Result<bool>.Fail("المنتج غير موجود", "NotFound", ResultStatus.NotFound);

            // Delete all media
            var mediaList = await _mediaRepository.GetMediaByItemIdAsync(request.Id);
            foreach (var media in mediaList)
                await _mediaRepository.DeleteMediaAsync(media.Id);

            // Delete all features
            // (Assume _featureRepository has a method to get and delete by productId)

            // Delete product
            // (Assume repository handles soft delete or cascade as needed)
            _productRepository.Remove(product);

            // Delete base item
            // (Assume product has a BaseItemId or similar linkage)
            // _baseItemRepository.RemoveById(product.BaseItemId); // Uncomment if such method exists

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransaction();
            return Result<bool>.Ok(true, "تم حذف المنتج بنجاح", ResultStatus.Success);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransaction();
            return Result<bool>.Fail($"فشل في حذف المنتج: {ex.Message}", "DeleteProductAggregateFailed", ResultStatus.Failed, ex);
        }
    }
}