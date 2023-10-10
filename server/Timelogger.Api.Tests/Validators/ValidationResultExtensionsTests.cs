using FluentAssertions;
using FluentValidation.Results;
using Timelogger.Api.Validators;
using Timelogger.Foundations.Errors;
using Xunit;

namespace Timelogger.Api.Tests.Validators;

public class ValidationResultExtensionsTests
{
    [Fact]
    public void ToFailedResult_ShouldThrowException_IfResultValid()
    {
        var validationResult = new ValidationResult();

        var action = () => validationResult.ToFailedResult<Result<object>>();

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ToFailedResult_ShouldReturn_NewFailedResult()
    {
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("", "error message 1"),
            new ValidationFailure("", "error message 2")
        });

        var result = validationResult.ToFailedResult<Result<object>>();

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo(new[]
            { new BadRequestError("error message 1"), new BadRequestError("error message 2") });
    }
}