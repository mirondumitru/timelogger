using AutoFixture;
using FluentAssertions;
using Timelogger.Api.Validators;
using Timelogger.Projects;
using Timelogger.TimeRegistrations;
using Xunit;

namespace Timelogger.Api.Tests.Validators;

public class TimeRegistrationsValidatorTests
{
    private readonly TimeRegistrationsValidator _sut;

    public TimeRegistrationsValidatorTests()
    {
        _sut = new TimeRegistrationsValidator();
    }

    [Fact]
    public async Task Should_Validate_Hours_Not_Zero()
    {
        var fixture = new Fixture();
        var model = fixture.Build<TimeRegistration>()
            .Without(x => x.CreatedAtUtc)
            .With(x => x.Hours, 0)
            .Create();

        var validationResult = await _sut.ValidateAsync(model);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Single().PropertyName.Should().Be("Hours");
    }

    [Fact]
    public async Task Should_Validate_Hours_GreaterThan_Zero()
    {
        var fixture = new Fixture();
        var model = fixture.Build<TimeRegistration>()
            .Without(x => x.CreatedAtUtc)
            .With(x => x.Hours, -1)
            .Create();

        var validationResult = await _sut.ValidateAsync(model);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Single().PropertyName.Should().Be("Hours");
    }

    [Fact]
    public async Task Should_Validate_Project_Exists()
    {
        var fixture = new Fixture();
        var model = fixture.Build<TimeRegistration>()
            .Without(x => x.CreatedAtUtc)
            .Without(x => x.Project)
            .Create();

        var validationResult = await _sut.ValidateAsync(model);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Single().PropertyName.Should().Be("Project");
    }

    [Fact]
    public async Task Should_Validate_ProjectId_Not_Zero()
    {
        var fixture = new Fixture();
        var project = fixture.Build<Project>().Without(x => x.Id).Create();
        var model = fixture.Build<TimeRegistration>()
            .Without(x => x.CreatedAtUtc)
            .With(x => x.Project, project)
            .Create();

        var validationResult = await _sut.ValidateAsync(model);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Single().PropertyName.Should().Be("Project.Id");
    }

    [Fact]
    public async Task Should_Validate_ValueDate_NotEmpty()
    {
        var fixture = new Fixture();
        var model = fixture.Build<TimeRegistration>()
            .Without(x => x.CreatedAtUtc)
            .Without(x => x.ValueDate)
            .Create();

        var validationResult = await _sut.ValidateAsync(model);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Single().PropertyName.Should().Be("ValueDate");
    }

    [Fact]
    public async Task Should_Validate_CreatedAt_Not_Set()
    {
        var fixture = new Fixture();
        var model = fixture.Build<TimeRegistration>().Create();

        var validationResult = await _sut.ValidateAsync(model);

        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Single().PropertyName.Should().Be("CreatedAtUtc");
    }

    [Fact]
    public async Task Should_Validate_Model()
    {
        var fixture = new Fixture();
        var model = fixture.Build<TimeRegistration>()
            .With(x => x.Hours, 5)
            .Without(x => x.CreatedAtUtc).Create();

        var validationResult = await _sut.ValidateAsync(model);

        validationResult.IsValid.Should().BeTrue();
    }
}