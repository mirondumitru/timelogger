using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Timelogger.Foundations.DateTime;
using Timelogger.Foundations.Errors;
using Timelogger.Projects;

namespace Timelogger.TimeRegistrations.Commands.Handlers.cs;

internal class CreateTimeRegistrationCommandHandler : IRequestHandler<CreateTimeRegistrationCommand, Result<TimeRegistration>>
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IProjectsRepository _projectsRepository;
    private readonly ITimeRegistrationsRepository _timeRegistrationsRepository;

    public CreateTimeRegistrationCommandHandler(ITimeRegistrationsRepository timeRegistrationsRepository,
        IProjectsRepository projectsRepository, IDateTimeService dateTimeService)
    {
        _timeRegistrationsRepository = timeRegistrationsRepository;
        _projectsRepository = projectsRepository;
        _dateTimeService = dateTimeService;
    }

    public async Task<Result<TimeRegistration>> Handle(CreateTimeRegistrationCommand request,
        CancellationToken cancellationToken)
    {
        var timeRegistration = request.TimeRegistration;

        if (timeRegistration.Hours < 0.5m)
            return new Result<TimeRegistration>(new BadRequestError("Cannot register less then 30 minutes"));

        var project = await _projectsRepository.GetAsync(timeRegistration.Project.Id);

        if (project == null)
            return new Result<TimeRegistration>(
                new NotFoundError($"Project with ID '{timeRegistration.Project.Id}' was not found"));

        if (project.IsCompleted)
            return new Result<TimeRegistration>(
                new BadRequestError(
                    $"Cannot register time - Project with ID '{timeRegistration.Project.Id}' is completed"));

        timeRegistration.CreatedAtUtc = _dateTimeService.UtcNow;
        timeRegistration.Project = project;

        var savedTimeRegistration = await _timeRegistrationsRepository.SaveAsync(timeRegistration);

        return new Result<TimeRegistration>(savedTimeRegistration);
    }
}