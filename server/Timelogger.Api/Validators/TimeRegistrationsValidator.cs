using System;
using FluentValidation;
using Timelogger.TimeRegistrations;

namespace Timelogger.Api.Validators;

public class TimeRegistrationsValidator : AbstractValidator<TimeRegistration>
{
    public TimeRegistrationsValidator()
    {
        RuleFor(x => x.Project)
            .NotNull().WithMessage("'Project' is required");

        RuleFor(x => x.Project.Id)
            .NotEqual(0).When(x => x.Project != null)
            .WithMessage("'Project.Id' is required");

        RuleFor(x => x.ValueDate)
            .NotNull()
            .NotEqual(default(DateTime))
            .WithMessage("'ValueDate' is required");

        RuleFor(x => x.Hours)
            .NotNull()
            .NotEqual(default(int))
            .WithMessage("'Hours' is required");

        RuleFor(x => x.Hours)
            .GreaterThan(0)
            .When(x => x.Hours != 0)
            .WithMessage("'Hours' should be greater than 0");

        RuleFor(x => x.CreatedAtUtc)
            .Null().WithMessage("'CreatedAtUtc' cannot be manually set and should be empty");
    }
}