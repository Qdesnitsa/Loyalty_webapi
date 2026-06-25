using Loyalty.Api.Contracts.Programs;
using Loyalty.Api.Controllers;
using Loyalty.Application.Programs;
using Loyalty.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Shouldly;
using ApplicationProgram = Loyalty.Application.Programs.Models.Program;

namespace Loyalty.Api.UnitTests;

public class ProgramsControllerTests
{

    [Fact]
    public async Task GetAll_WhenNoPrograms_ReturnsOkWithEmptyList()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetProgramsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ApplicationProgram>>(Array.Empty<ApplicationProgram>()));

        var controller = new ProgramsController(mediator);

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<GetProgramsResponse>();
        response.Programs.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAll_WhenProgramsExist_ReturnsOkWithPrograms()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var program = ApplicationProgram.FromDomain(ProgramMocks.Default);
        mediator.Send(Arg.Any<GetProgramsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ApplicationProgram>>([program]));

        var controller = new ProgramsController(mediator);

        // Act
        var result = await controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<GetProgramsResponse>();
        response.Programs.Length.ShouldBe(1);
        response.Programs[0].Id.ShouldBe(program.Id);
    }

    [Fact]
    public async Task GetById_ReturnsProgram()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var program = ApplicationProgram.FromDomain(ProgramMocks.Default);
        mediator.Send(Arg.Any<GetProgramQuery>(), Arg.Any<CancellationToken>())
            .Returns(program);

        var controller = new ProgramsController(mediator);

        // Act
        var result = await controller.Get(program.Id, CancellationToken.None);

        // Assert
        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<GetProgramResponse>();
        response.Program.Id.ShouldBe(program.Id);
    }

    [Fact]
    public async Task Create_ReturnsCreated()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var program = ApplicationProgram.FromDomain(ProgramMocks.Default);
        mediator.Send(Arg.Any<CreateProgramCommand>(), Arg.Any<CancellationToken>())
            .Returns(program);

        var controller = new ProgramsController(mediator);
        var request = CreateProgramRequest();

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var createdResult = result.Result.ShouldBeOfType<CreatedAtActionResult>();
        createdResult.ActionName.ShouldBe(nameof(ProgramsController.Get));
        var response = createdResult.Value.ShouldBeOfType<CreateProgramResponse>();
        response.Program.Id.ShouldBe(program.Id);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var program = ApplicationProgram.FromDomain(ProgramMocks.Default);
        mediator.Send(Arg.Any<UpdateProgramCommand>(), Arg.Any<CancellationToken>())
            .Returns(program);

        var controller = new ProgramsController(mediator);
        var request = UpdateProgramRequest();

        // Act
        var result = await controller.Update(program.Id, request, CancellationToken.None);

        // Assert
        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<UpdateProgramResponse>();
        response.Program.Id.ShouldBe(program.Id);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<DeleteProgramCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var controller = new ProgramsController(mediator);
        var request = new DeleteProgramRequest("test-user");

        // Act
        var result = await controller.Delete("default-program", request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();
    }

    private static CreateProgramRequest CreateProgramRequest() =>
        new(
            Title: "Default Program",
            Description: "Default program description",
            State: ProgramState.Active,
            StartDate: DateTime.UtcNow.AddDays(1),
            FinishDate: DateTime.UtcNow.AddDays(30),
            MinTransactionAmount: 100,
            MaxTransactionAmount: 1000,
            TransactionType: "Deposit",
            Achievement: new CreateAchievementRequest(
                1,
                new CreateRewardRequest(10, RewardValueType.Percent, RewardTarget.Bonus, RewardValueUsageType.Add)),
            CreatedBy: "test-user");

    private static UpdateProgramRequest UpdateProgramRequest() =>
        new(
            Title: "Updated Program",
            Description: "Updated description",
            State: ProgramState.Active,
            StartDate: DateTime.UtcNow.AddDays(1),
            FinishDate: DateTime.UtcNow.AddDays(30),
            MinTransactionAmount: 100,
            MaxTransactionAmount: 1000,
            TransactionType: "Deposit",
            Achievement: new CreateAchievementRequest(
                1,
                new CreateRewardRequest(10, RewardValueType.Percent, RewardTarget.Bonus, RewardValueUsageType.Add)),
            UpdatedBy: "test-user");

}
