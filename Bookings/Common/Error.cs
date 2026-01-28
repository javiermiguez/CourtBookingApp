namespace Bookings.Common;

public record Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    public Error(string code, string message, ErrorType type = ErrorType.Validation)
    {
        Code = code;
        Message = message;
        Type = type;
    }
}

public enum ErrorType
{
    None = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5
}
