using System;

namespace AlSaad.Core.Common;

/// <summary>
/// يمثل نتيجة عملية ما
/// </summary>
public class Result<T>
{
    public bool Success { get; private set; }
    public string Message { get; private set; }
    public T Data { get; private set; }
    public Exception Exception { get; private set; }

    private Result(bool success, string message, T data = default, Exception exception = null)
    {
        Success = success;
        Message = message;
        Data = data;
        Exception = exception;
    }

    public static Result<T> Ok(T data, string message = "تمت العملية بنجاح")
        => new(true, message, data);

    public static Result<T> Fail(string message)
        => new(false, message);

    public static Result<T> Fail(Exception exception, string message = "حدث خطأ غير متوقع")
        => new(false, message, exception: exception);
}

/// <summary>
/// يمثل نتيجة عملية بدون بيانات
/// </summary>
public class Result
{
    public bool Success { get; private set; }
    public string Message { get; private set; }
    public Exception Exception { get; private set; }

    private Result(bool success, string message, Exception exception = null)
    {
        Success = success;
        Message = message;
        Exception = exception;
    }

    public static Result Ok(string message = "تمت العملية بنجاح")
        => new(true, message);

    public static Result Fail(string message)
        => new(false, message);

    public static Result Fail(Exception exception, string message = "حدث خطأ غير متوقع")
        => new(false, message, exception);
}
