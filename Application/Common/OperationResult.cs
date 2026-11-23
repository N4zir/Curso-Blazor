namespace CumbreSalonesCongresos.Application.Common;

public record OperationResult(bool Ok, string? Error = null)
{
    public static OperationResult Success() => new(true);
    public static OperationResult Fail(string error) => new(false, error);
}

public record OperationResult<T>(bool Ok, T? Value, string? Error = null)
{
    public static OperationResult<T> Success(T value) => new(true, value, null);
    public static OperationResult<T> Fail(string error) => new(false, default, error);
}
