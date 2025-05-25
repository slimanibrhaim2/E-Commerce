using Core.Result;
using System;

namespace Catalogs.Application.Commands.AddMedia;

public class AddMediaCommandValidator
{
    public Result Validate(AddMediaCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Mediadto.Url))
            return Result.Fail(
                message: "رابط الوسائط مطلوب",
                errorType: "ValidationError",
                resultStatus: Core.Result.ResultStatus.ValidationError);

        if (command.Mediadto.Url.Length > 500)
            return Result.Fail(
                message: "رابط الوسائط يجب أن لا يتجاوز 500 حرف",
                errorType: "ValidationError",
                resultStatus: Core.Result.ResultStatus.ValidationError);

        if (command.Mediadto.MediaTypeId == Guid.Empty)
            return Result.Fail(
                message: "معرف نوع الوسائط مطلوب",
                errorType: "ValidationError",
                resultStatus: Core.Result.ResultStatus.ValidationError);

        if (command.ItemId == Guid.Empty)
            return Result.Fail(
                message: "معرف العنصر مطلوب",
                errorType: "ValidationError",
                resultStatus: Core.Result.ResultStatus.ValidationError);

        return Result.Ok(resultStatus: Core.Result.ResultStatus.Success);
    }
} 