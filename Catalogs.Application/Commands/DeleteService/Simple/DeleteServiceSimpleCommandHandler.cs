using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Commands.DeleteService.Simple;

public class DeleteServiceSimpleCommandHandler : IRequestHandler<DeleteServiceSimpleCommand, Result<bool>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<DeleteServiceSimpleCommandHandler> _logger;

    public DeleteServiceSimpleCommandHandler(IServiceRepository repo, ILogger<DeleteServiceSimpleCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteServiceSimpleCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (request.Id == Guid.Empty)
        {
            return Result<bool>.Fail(
                message: "معرف الخدمة مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            var service = await _repo.GetByIdAsync(request.Id);
            if (service == null)
                return Result<bool>.Fail(
                    message: "الخدمة غير موجودة",
                    errorType: "ServiceNotFound",
                    resultStatus: ResultStatus.NotFound);

            _repo.Remove(service);

            return Result<bool>.Ok(
                data: true,
                message: "تم حذف الخدمة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting service");
            return Result<bool>.Fail(
                message: "حدث خطأ أثناء حذف الخدمة",
                errorType: "DeleteServiceFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
}