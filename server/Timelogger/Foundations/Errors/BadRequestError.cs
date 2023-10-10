namespace Timelogger.Foundations.Errors;

public class BadRequestError : IError
{
    public BadRequestError(string message)
    {
        Message = message;
    }

    public string Message { get; }
}