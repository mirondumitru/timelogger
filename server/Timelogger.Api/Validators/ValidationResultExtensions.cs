using System;
using System.Linq;
using FluentValidation.Results;
using Timelogger.Foundations.Errors;

namespace Timelogger.Api.Validators;

public static class ValidationResultExtensions
{
    public static Result<T> ToFailedResult<T>(this ValidationResult result)
    {
        if (result.IsValid)
        {
            throw new ArgumentException("Cannot transform valid result");
        }

        var errors = result.Errors.Select(x => new BadRequestError(x.ErrorMessage));

        return new Result<T>(errors);
    }
}