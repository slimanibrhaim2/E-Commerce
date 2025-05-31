using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Interfaces;

namespace Catalogs.Application.Commands.DeleteProduct.Simple;

public class DeleteProductSimpleCommandHandler : IRequestHandler<DeleteProductSimpleCommand, Result<bool>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<DeleteProductSimpleCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductSimpleCommandHandler(IProductRepository repo, ILogger<DeleteProductSimpleCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteProductSimpleCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (request.Id == Guid.Empty)
        {
            return Result<bool>.Fail(
                message: "معرف المنتج مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            var product = await _repo.GetByIdAsync(request.Id);
            if (product == null)
                return Result<bool>.Fail(
                    message: "المنتج غير موجود",
                    errorType: "ProductNotFound",
                    resultStatus: ResultStatus.NotFound);

            _repo.Remove(product);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(
                data: true,
                message: "تم حذف المنتج بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product");
            return Result<bool>.Fail(
                message: "حدث خطأ أثناء حذف المنتج",
                errorType: "DeleteProductFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
}