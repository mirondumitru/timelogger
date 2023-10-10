namespace Timelogger.Foundations.Errors;

public class NotFoundError : IError
{
    public NotFoundError(string message)
    {
        Message = message;
    }

    public string Message { get; }
}