using System.Runtime.CompilerServices;

namespace Bookings.Common;

public static class EnumParser
{
    public static Result<T> TryParse<T>(string value, [CallerArgumentExpression("value")] string parameterName = "")
        where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, out var result))
        {
            return Result.Success(result);
        }

        var error = new Error(
            $"Invalid{typeof(T).Name}",
            $"Invalid {typeof(T).Name} value '{value}' for parameter '{parameterName}'");

        return Result.Failure<T>(error);
    }

    public static bool TryParse<T>(string value, out T result, out Error error)
        where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, out result))
        {
            error = Error.None;
            return true;
        }

        error = new Error(
            $"Invalid{typeof(T).Name}",
            $"Invalid {typeof(T).Name}: {value}");

        return false;
    }
}
