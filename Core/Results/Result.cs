using System.Text.Json.Serialization;

namespace Core.Result;

public class Result<T> : Result
{
    public T Data { get; set; }

    public Result(
        bool success,
        ResultStatus resultStatus,
        T data, string? message,
        string? errorType,
        Exception? exception = null)
        : base(success, resultStatus, message, errorType, exception)
    {
        Data = data;
    }

    public static Result<T> Ok(T data, string? message, ResultStatus resultStatus)
        => new Result<T>(true, resultStatus, data, message, null);
    public new static Result<T> Fail(
        string? message,
         string? errorType,
          ResultStatus resultStatus,
          Exception? exception = null)
        => new Result<T>(false, resultStatus, default!, message, errorType, exception);
}


public class Result
{
    [JsonIgnore]
    public Exception? Exception { get; set; } = null;
    public ResultStatus ResultStatus { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ErrorType { get; set; }
    public Result(bool success,
    ResultStatus resultStatus,
    string? message,
    string? errorType,
    Exception? exception = null)
    {
        Exception = exception;
        Success = success;
        Message = message;
        ErrorType = errorType;
        ResultStatus = resultStatus;
    }

    public static Result Ok(ResultStatus resultStatus) => new Result(true, resultStatus, null, null);
    public static Result Ok(string? message, ResultStatus resultStatus) => new Result(true, resultStatus, message, null);
    public static Result Fail(string? message,
        string? errorType,
        ResultStatus resultStatus,
        Exception? exception = null) => new Result(false, resultStatus, message, errorType, exception);
}


public enum ResultStatus
{
    Success = 1,
    ValidationError = 2,
    InternalServerError = 3,
    Failed = 4
}
