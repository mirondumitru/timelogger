using System.Collections.Generic;
using System.Linq;

namespace Timelogger.Foundations.Errors;

public class Result<T>
{
    public Result(T value)
    {
        Value = value;
    }

    public Result(IError error)
    {
        Errors = new[] { error };
    }

    public Result(IEnumerable<IError> errors)
    {
        Errors = errors;
    }

    public bool IsSuccess => Value != null && (Errors == null || !Errors.Any());
    public T Value { get; }
    public IEnumerable<IError> Errors { get; }

    public bool HasErrors<TError>() where TError : IError
    {
        return Errors.Any(e => e is TError);
    }
}