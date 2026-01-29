namespace Users.Common;

public class Result
{
    public bool IsSuccess { get; }
    public Error Error { get; }

    public Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

public class Result<T> : Result
{
    public T Value { get; }

    public Result(T value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }
}
