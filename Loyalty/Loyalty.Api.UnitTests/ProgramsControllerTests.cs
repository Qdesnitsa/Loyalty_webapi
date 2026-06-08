using Loyalty.Api.Contracts.Programs;
using Loyalty.Api.Controllers;
using Loyalty.Application.Programs;
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
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetProgramsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<ApplicationProgram>>(Array.Empty<ApplicationProgram>()));

        var controller = new ProgramsController(mediator);

        var result = await controller.GetAll(CancellationToken.None);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<GetProgramsResponse>();
        response.Programs.ShouldBeEmpty();
    }
}
