using AutoFixture;
using FluentAssertions;
using Moq;
using Timelogger.Foundations.DateTime;
using Timelogger.Foundations.Errors;
using Timelogger.Projects;
using Timelogger.TimeRegistrations;
using Timelogger.TimeRegistrations.Commands;
using Timelogger.TimeRegistrations.Commands.Handlers.cs;
using Xunit;

namespace Timelogger.Tests.TimeRegistrations;

public class CreateTimeRegistrationCommandHandlerTests : IDisposable
{
    private readonly Mock<IDateTimeService> _mockDateTimeService;
    private readonly Mock<IProjectsRepository> _mockProjectsRepository;
    private readonly MockRepository _mockRepository;
    private readonly Mock<ITimeRegistrationsRepository> _mockTimeRegistrationsRepository;
    private readonly CreateTimeRegistrationCommandHandler _sut;

    public CreateTimeRegistrationCommandHandlerTests()
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);

        _mockTimeRegistrationsRepository = _mockRepository.Create<ITimeRegistrationsRepository>();
        _mockProjectsRepository = _mockRepository.Create<IProjectsRepository>();
        _mockDateTimeService = _mockRepository.Create<IDateTimeService>();

        _sut = new CreateTimeRegistrationCommandHandler(_mockTimeRegistrationsRepository.Object,
            _mockProjectsRepository.Object,
            _mockDateTimeService.Object);
    }


    public void Dispose()
    {
        _mockRepository.VerifyAll();
    }

    [Fact]
    public async Task Handle_ShouldReturn_BadRequest_IfMinutesInvalid()
    {
        // arrange
        var fixture = new Fixture();

        var timeRegistration = fixture.Build<TimeRegistration>()
            .With(x => x.Minutes, 20)
            .Create();

        // act
        var result = await _sut.Handle(new CreateTimeRegistrationCommand(timeRegistration), default);

        // assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Single().Should().BeOfType<BadRequestError>();
        result.Errors.Single().As<BadRequestError>().Message.Should().Be("Cannot register less then 30 minutes");
    }

    [Fact]
    public async Task Handle_ShouldReturn_BadRequest_IfProjectNotFound()
    {
        // arrange
        var fixture = new Fixture();
        var project = fixture.Create<Project>();

        var timeRegistration = fixture.Build<TimeRegistration>()
            .With(x => x.Minutes, 60)
            .With(x => x.Project, project)
            .Create();

        _mockProjectsRepository.Setup(x => x.GetAsync(project.Id)).ReturnsAsync((Project?)null);

        // act
        var result = await _sut.Handle(new CreateTimeRegistrationCommand(timeRegistration), default);

        // assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Single().Should().BeOfType<BadRequestError>();
        result.Errors.Single().As<BadRequestError>().Message.Should()
            .Be($"Project with ID '{project.Id}' was not found");
    }

    [Fact]
    public async Task Handle_ShouldReturn_BadRequest_IfProjectCompleted()
    {
        // arrange
        var fixture = new Fixture();
        var project = fixture.Build<Project>()
            .With(x => x.IsCompleted, true).Create();

        var timeRegistration = fixture.Build<TimeRegistration>()
            .With(x => x.Minutes, 60)
            .With(x => x.Project, project)
            .Create();

        _mockProjectsRepository.Setup(x => x.GetAsync(project.Id)).ReturnsAsync(project);

        // act
        var result = await _sut.Handle(new CreateTimeRegistrationCommand(timeRegistration), default);

        // assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Single().Should().BeOfType<BadRequestError>();
        result.Errors.Single().As<BadRequestError>().Message.Should()
            .Be($"Cannot register time - Project with ID '{project.Id}' is completed");
    }

    [Fact]
    public async Task Handle_ShouldInvoke_RepositorySave()
    {
        // arrange
        var fixture = new Fixture();
        var utcNow = fixture.Create<DateTime>();
        var project = fixture.Build<Project>()
            .With(x => x.IsCompleted, false).Create();

        var timeRegistration = fixture.Build<TimeRegistration>()
            .With(x => x.Minutes, 60)
            .With(x => x.Project, project)
            .Create();

        _mockDateTimeService.Setup(x => x.UtcNow).Returns(utcNow);
        _mockProjectsRepository.Setup(x => x.GetAsync(project.Id)).ReturnsAsync(project);
        _mockTimeRegistrationsRepository.Setup(x => x.SaveAsync(timeRegistration)).ReturnsAsync(timeRegistration);

        // act
        var result = await _sut.Handle(new CreateTimeRegistrationCommand(timeRegistration), default);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CreatedAtUtc.Should().Be(utcNow);
    }
}