using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Timelogger.Api.Controllers;
using Timelogger.Foundations.Errors;
using Timelogger.TimeRegistrations;
using Timelogger.TimeRegistrations.Commands;
using Xunit;

namespace Timelogger.Api.Tests.Controllers;

public class TimeRegistrationsControllerTests : IDisposable
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly MockRepository _mockRepository;
    private readonly Mock<ITimeRegistrationsRepository> _mockTimeRegistrationsRepository;
    private readonly Mock<IValidator<TimeRegistration>> _mockValidator;
    private readonly TimeRegistrationsController _sut;

    public TimeRegistrationsControllerTests()
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);

        _mockMediator = _mockRepository.Create<IMediator>();
        _mockValidator = _mockRepository.Create<IValidator<TimeRegistration>>();
        _mockTimeRegistrationsRepository = _mockRepository.Create<ITimeRegistrationsRepository>();

        _sut = new TimeRegistrationsController(_mockMediator.Object, _mockValidator.Object,
            _mockTimeRegistrationsRepository.Object);
    }

    [Fact]
    public async Task Post_ShouldReturn_BadRequest_ForInvalidModel()
    {
        // arrange
        var fixture = new Fixture();
        var instance = fixture.Create<TimeRegistration>();

        _mockValidator.Setup(x => x.ValidateAsync(instance, CancellationToken.None))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Hours", "Error message") }));

        // act
        var result = await _sut.Post(instance);

        // assert
        result.Should().BeOfType<BadRequestObjectResult>();
        result.As<BadRequestObjectResult>().Value.Should().BeOfType<Result<TimeRegistration>>();
        result.As<BadRequestObjectResult>().Value.As<Result<TimeRegistration>>().IsSuccess.Should().BeFalse();
        result.As<BadRequestObjectResult>().Value.As<Result<TimeRegistration>>()
            .Errors.Should().OnlyContain(x => x.Message == "Error message");
    }

    [Fact]
    public async Task Post_ShouldReturn_Ok_ForSuccessfulCommandExecution()
    {
        // arrange
        var fixture = new Fixture();
        var instance = fixture.Create<TimeRegistration>();

        _mockValidator.Setup(x => x.ValidateAsync(instance, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateTimeRegistrationCommand>(),default))
            .ReturnsAsync((CreateTimeRegistrationCommand command, CancellationToken _) => new Result<TimeRegistration>(command.TimeRegistration));

        // act
        var result = await _sut.Post(instance);

        // assert
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.Should().BeOfType<TimeRegistration>();
        result.As<OkObjectResult>().Value.As<TimeRegistration>().Should().Be(instance);
    }

    [Fact]
    public async Task Post_ShouldReturn_BadRequest_ForFailedResult()
    {
        // arrange
        var fixture = new Fixture();
        var instance = fixture.Create<TimeRegistration>();

        _mockValidator.Setup(x => x.ValidateAsync(instance, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateTimeRegistrationCommand>(), default))
            .ReturnsAsync(new Result<TimeRegistration>(new BadRequestError("bad request message")));

        // act
        var result = await _sut.Post(instance);

        // assert
        result.Should().BeOfType<BadRequestObjectResult>();
        result.As<BadRequestObjectResult>().Value.Should().BeOfType<Result<TimeRegistration>>();
        result.As<BadRequestObjectResult>().Value.As<Result<TimeRegistration>>().IsSuccess.Should().BeFalse();
        result.As<BadRequestObjectResult>().Value.As<Result<TimeRegistration>>()
            .Errors.Should().OnlyContain(x => x.Message == "bad request message");
    }

    [Fact]
    public async Task Post_ShouldReturn_NotFound_ForFailedResult()
    {
        // arrange
        var fixture = new Fixture();
        var instance = fixture.Create<TimeRegistration>();

        _mockValidator.Setup(x => x.ValidateAsync(instance, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateTimeRegistrationCommand>(), default))
            .ReturnsAsync(new Result<TimeRegistration>(new NotFoundError("not found message")));

        // act
        var result = await _sut.Post(instance);

        // assert
        result.Should().BeOfType<NotFoundObjectResult>();
        result.As<NotFoundObjectResult>().Value.Should().BeOfType<Result<TimeRegistration>>();
        result.As<NotFoundObjectResult>().Value.As<Result<TimeRegistration>>().IsSuccess.Should().BeFalse();
        result.As<NotFoundObjectResult>().Value.As<Result<TimeRegistration>>()
            .Errors.Should().OnlyContain(x => x.Message == "not found message");
    }

    [Fact]
    public async Task Get_ShouldInvoke_Repository()
    {
        // arrange
        var fixture = new Fixture();
        var projectId = fixture.Create<int>();
        var timeRegistrations = fixture.CreateMany<TimeRegistration>().ToList();

        _mockTimeRegistrationsRepository.Setup(x => x.Get(projectId)).ReturnsAsync(timeRegistrations);

        // act
        var result = await _sut.Get(projectId);

        // assert
        result.Should().BeOfType<OkObjectResult>();
        result.As<OkObjectResult>().Value.Should().BeAssignableTo<IEnumerable<TimeRegistration>>();
        result.As<OkObjectResult>().Value.As<IEnumerable<TimeRegistration>>().Should().BeEquivalentTo(timeRegistrations);
    }

    public void Dispose()
    {
        _mockRepository.VerifyAll();
        _sut.Dispose();
    }
}