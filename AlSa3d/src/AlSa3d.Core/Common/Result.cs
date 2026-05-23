using System;
using System.Collections.Generic;

namespace AlSa3d.Core;

/// <summary>
/// نتيجة عملية عامة بدون بيانات
/// </summary>
public class Result
{
    /// <summary>
    /// هل كانت العملية ناجحة
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// رسالة نصية توضيحية للنتيجة
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// الاستثناء إن وجد
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// قائمة بالأخطاء التفصيلية
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// اختصار للتحقق من النجاح
    /// </summary>
    public bool IsSuccess => Success;

    /// <summary>
    /// اختصار للتحقق من الفشل
    /// </summary>
    public bool IsFailed => !Success;

    /// <summary>
    /// إرجاع نتيجة ناجحة
    /// </summary>
    public static Result SuccessResult() => new() { Success = true, Message = "تم بنجاح" };

    /// <summary>
    /// إرجاع نتيجة فشل
    /// </summary>
    public static Result Failure(string message) => new() { Success = false, Message = message };

    /// <summary>
    /// إرجاع نتيجة فشل مع استثناء
    /// </summary>
    public static Result Failure(string message, Exception ex) => new() 
    { 
        Success = false, 
        Message = message, 
        Exception = ex 
    };

    /// <summary>
    /// إرجاع نتيجة فشل مع أخطاء متعددة
    /// </summary>
    public static Result Failure(params string[] errors) => new() 
    { 
        Success = false, 
        Message = "فشلت العملية",
        Errors = new List<string>(errors)
    };

    /// <summary>
    /// إرجاع نتيجة ناجحة مع بيانات
    /// </summary>
    public static Result<T> Ok<T>(T data) => new() { Success = true, Data = data, Message = "تم بنجاح" };

    /// <summary>
    /// إرجاع نتيجة فشل مع بيانات
    /// </summary>
    public static Result<T> Failure<T>(string message) => new() { Success = false, Message = message };

    /// <summary>
    /// إرجاع نتيجة فشل مع استثناء وبيانات
    /// </summary>
    public static Result<T> Failure<T>(string message, Exception ex) => new() 
    { 
        Success = false, 
        Message = message, 
        Exception = ex 
    };
}

/// <summary>
/// نتيجة عملية تحتوي على بيانات من نوع T
/// </summary>
/// <typeparam name="T">نوع البيانات المرجعة</typeparam>
public class Result<T>
{
    /// <summary>
    /// هل كانت العملية ناجحة
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// رسالة نصية توضيحية للنتيجة
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// البيانات المرجعة من العملية
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// الاستثناء إن وجد
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// قائمة بالأخطاء التفصيلية
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// اختصار للتحقق من النجاح
    /// </summary>
    public bool IsSuccess => Success;

    /// <summary>
    /// اختصار للتحقق من الفشل
    /// </summary>
    public bool IsFailed => !Success;

    /// <summary>
    /// إرجاع نتيجة ناجحة مع بيانات
    /// </summary>
    public static Result<T> SuccessResult(T data) => new() { Success = true, Data = data, Message = "تم بنجاح" };

    /// <summary>
    /// إرجاع نتيجة فشل
    /// </summary>
    public static Result<T> Failure(string message) => new() { Success = false, Message = message, Data = default };

    /// <summary>
    /// إرجاع نتيجة فشل مع استثناء
    /// </summary>
    public static Result<T> Failure(string message, Exception ex) => new() 
    { 
        Success = false, 
        Message = message, 
        Exception = ex,
        Data = default 
    };

    /// <summary>
    /// إرجاع نتيجة فشل مع أخطاء متعددة
    /// </summary>
    public static Result<T> Failure(params string[] errors) => new() 
    { 
        Success = false, 
        Message = "فشلت العملية",
        Errors = new List<string>(errors),
        Data = default 
    };
}
