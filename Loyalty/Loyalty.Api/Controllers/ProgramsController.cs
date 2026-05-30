using Loyalty.Api.Contracts.Programs;
using Loyalty.Application.Programs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ApiProgram = Loyalty.Api.Contracts.Programs.Program;
using DomainAchievement = Loyalty.Domain.Entities.Achievement;
using DomainReward = Loyalty.Domain.Entities.Reward;

namespace Loyalty.Api.Controllers;

[ApiController]
[Route("api/programs")]
public sealed class ProgramsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateProgramResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateProgramResponse>> Create(
        [FromBody] CreateProgramRequest request,
        CancellationToken cancellationToken)
    {
        var program = await mediator.Send(ToCommand(request), cancellationToken);
        var response = new CreateProgramResponse(ApiProgram.FromApplication(program));

        return CreatedAtAction(nameof(Get), new { id = program.Id }, response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetProgramResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProgramResponse>> Get(string id, CancellationToken cancellationToken)
    {
        var program = await mediator.Send(new GetProgramQuery(id), cancellationToken);
        return Ok(new GetProgramResponse(ApiProgram.FromApplication(program)));
    }

    private static CreateProgramCommand ToCommand(CreateProgramRequest request) =>
        new(
            request.Id,
            request.Title,
            request.Description,
            request.State,
            request.StartDate,
            request.FinishDate,
            request.MinTransactionAmount,
            request.MaxTransactionAmount,
            new DomainAchievement
            {
                Id = request.Achievement.Id,
                TransactionsCountToApplyAchievement = request.Achievement.TransactionsCountToApplyAchievement ?? 0,
                Reward = new DomainReward
                {
                    Id = request.Achievement.Reward.Id,
                    Amount = request.Achievement.Reward.Amount,
                    Type = request.Achievement.Reward.Type,
                    Target = request.Achievement.Reward.Target,
                    UsageType = request.Achievement.Reward.UsageType
                }
            },
            request.CreatedBy);
}
