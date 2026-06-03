namespace NoteBook.Domain.ValueObjects;

/// <summary>
/// Generic Result value object for handling success/failure outcomes
/// Enables functional-style error handling
/// </summary>
public class Result<T>
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// The resulting value (only valid if IsSuccess is true)
    /// </summary>
    public T? Value { get; }
    
    /// <summary>
    /// Error message (only valid if IsSuccess is false)
    /// </summary>
    public string? Error { get; }
    
    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
    
    /// <summary>
    /// Create a successful result
    /// </summary>
    public static Result<T> Success(T value) => new(true, value, null);
    
    /// <summary>
    /// Create a failed result
    /// </summary>
    public static Result<T> Failure(string error) => new(false, default, error);
    
    /// <summary>
    /// Transform the value if successful
    /// </summary>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapping) =>
        IsSuccess 
            ? Result<TNew>.Success(mapping(Value!)) 
            : Result<TNew>.Failure(Error!);
    
    /// <summary>
    /// Chain operations
    /// </summary>
    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> mapping) =>
        IsSuccess 
            ? mapping(Value!) 
            : Result<TNew>.Failure(Error!);
    
    /// <summary>
    /// Execute action if successful
    /// </summary>
    public Result<T> Tap(Action<T> action)
    {
        if (IsSuccess)
            action(Value!);
        return this;
    }
    
    /// <summary>
    /// Get value or throw exception
    /// </summary>
    public T GetValueOrThrow() =>
        IsSuccess 
            ? Value! 
            : throw new InvalidOperationException($"Result failed: {Error}");
    
    /// <summary>
    /// Get value or return default
    /// </summary>
    public T? GetValueOrDefault() => Value;
}

/// <summary>
/// Generic Result value object for void operations
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// Error message (only valid if IsSuccess is false)
    /// </summary>
    public string? Error { get; }
    
    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
    
    /// <summary>
    /// Create a successful result
    /// </summary>
    public static Result Success() => new(true, null);
    
    /// <summary>
    /// Create a failed result
    /// </summary>
    public static Result Failure(string error) => new(false, error);
    
    /// <summary>
    /// Execute action if successful
    /// </summary>
    public Result Tap(Action action)
    {
        if (IsSuccess)
            action();
        return this;
    }
    
    /// <summary>
    /// Throw exception if failed
    /// </summary>
    public void ThrowIfFailed()
    {
        if (!IsSuccess)
            throw new InvalidOperationException($"Result failed: {Error}");
    }
}
