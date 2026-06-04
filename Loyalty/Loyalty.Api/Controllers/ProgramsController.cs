using Loyalty.Api.Contracts.Programs;
using Loyalty.Application.Programs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiProgram = Loyalty.Api.Contracts.Programs.Program;
using DomainAchievement = Loyalty.Domain.Entities.Achievement;
using DomainReward = Loyalty.Domain.Entities.Reward;

namespace Loyalty.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/programs")]
public sealed class ProgramsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateProgramResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateProgramResponse>> Create(
        [FromBody] CreateProgramRequest request,
        CancellationToken cancellationToken)
    {
        var program = await mediator.Send(ToCommand(request), cancellationToken);
        var response = new CreateProgramResponse(ApiProgram.FromApplication(program));

        return CreatedAtAction(nameof(Get), new { id = program.Id }, response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateProgramResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateProgramResponse>> Update(
        string id,
        [FromBody] UpdateProgramRequest request,
        CancellationToken cancellationToken)
    {
        var program = await mediator.Send(ToCommand(id, request), cancellationToken);
        return Ok(new UpdateProgramResponse(ApiProgram.FromApplication(program)));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        string id,
        [FromBody] DeleteProgramRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProgramCommand(id, request.UpdatedBy), cancellationToken);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetProgramsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetProgramsResponse>> GetAll(CancellationToken cancellationToken)
    {
        var programs = await mediator.Send(new GetProgramsQuery(), cancellationToken);
        return Ok(new GetProgramsResponse(programs.Select(ApiProgram.FromApplication).ToArray()));
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
            request.TransactionType,
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

    private static UpdateProgramCommand ToCommand(string id, UpdateProgramRequest request) =>
        new(
            id,
            request.Title,
            request.Description,
            request.State,
            request.StartDate,
            request.FinishDate,
            request.MinTransactionAmount,
            request.MaxTransactionAmount,
            request.TransactionType,
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
            request.UpdatedBy);
}
