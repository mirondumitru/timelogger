using MediatR;
using Timelogger.Foundations.Errors;

namespace Timelogger.TimeRegistrations.Commands;

public class CreateTimeRegistrationCommand : IRequest<Result<TimeRegistration>>
{
    public CreateTimeRegistrationCommand(TimeRegistration timeRegistration)
    {
        TimeRegistration = timeRegistration;
    }

    public TimeRegistration TimeRegistration { get; }
}