namespace AlSa3d.Core;

public class Result
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public bool IsSuccess => Success;

    public static Result SuccessResult() => new() { Success = true, Message = "تم بنجاح" };
    public static Result Failure(string message) => new() { Success = false, Message = message };

    public static Result<T> Ok<T>(T data) => new() { Success = true, Data = data, Message = "تم بنجاح" };
    public static Result<T> Failure<T>(string message) => new() { Success = false, Message = message };
}

public class Result<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public bool IsSuccess => Success;

    public static Result<T> SuccessResult(T data) => new() { Success = true, Data = data, Message = "تم بنجاح" };
    public static Result<T> Failure(string message) => new() { Success = false, Message = message, Data = default };
}
