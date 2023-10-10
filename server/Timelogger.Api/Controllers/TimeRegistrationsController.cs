using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Timelogger.Api.Validators;
using Timelogger.TimeRegistrations;
using Timelogger.TimeRegistrations.Commands;

namespace Timelogger.Api.Controllers;

[Route("api/[controller]")]
public class TimeRegistrationsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ITimeRegistrationsRepository _timeRegistrationsRepository;
    private readonly IValidator<TimeRegistration> _validator;

    public TimeRegistrationsController(IMediator mediator, IValidator<TimeRegistration> validator,
        ITimeRegistrationsRepository timeRegistrationsRepository)
    {
        _mediator = mediator;
        _validator = validator;
        _timeRegistrationsRepository = timeRegistrationsRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TimeRegistration timeRegistration)
    {
        var validationResult = await _validator.ValidateAsync(timeRegistration);

        if (!validationResult.IsValid)
        {
            var failedResult = validationResult.ToFailedResult<TimeRegistration>();

            return BadRequest(failedResult);
        }

        var result = await _mediator.Send(new CreateTimeRegistrationCommand(timeRegistration));

        return HttpResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? projectId)
    {
        var timeRegistrations = await _timeRegistrationsRepository.Get(projectId);

        return Ok(timeRegistrations);
    }
}