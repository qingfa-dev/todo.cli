public sealed class Result
{
    public bool Success { get; }

    public string? Error { get; }

    private Result(
        bool success,
        string? error)
    {
        Success = success;
        Error = error;
    }

    public static Result Ok()
    {
        return new Result(true, null);
    }

    public static Result Fail(string error)
    {
        return new Result(false, error);
    }
}

public sealed class Result<T>
{
    public bool Success { get; }

    public T? Value { get; }

    public string? Error { get; }

    private Result(
        bool success,
        T? value,
        string? error)
    {
        Success = success;
        Value = value;
        Error = error;
    }

    public static Result<T> Ok(T value)
    {
        return new Result<T>(
            true,
            value,
            null);
    }

    public static Result<T> Fail(string error)
    {
        return new Result<T>(
            false,
            default,
            error);
    }
}