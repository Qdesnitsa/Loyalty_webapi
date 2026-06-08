using Loyalty.Api.Contracts.Programs;
using Loyalty.Application.Programs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiProgram = Loyalty.Api.Contracts.Programs.Program;

namespace Loyalty.Api.Controllers;

/// <summary>Programs management API</summary>
/// <param name="mediator">Mediator</param>
[Authorize]
[ApiController]
[Route("api/programs")]
public sealed class ProgramsController(IMediator mediator) : ControllerBase
{
    /// <summary>Create a new program</summary>
    /// <param name="request">Request body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="201">New program</response>
    /// <response code="400">Invalid request</response>
    /// <returns>Created program</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateProgramResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateProgramResponse>> Create(
        [FromBody] CreateProgramRequest request,
        CancellationToken cancellationToken)
    {
        var program = await mediator.Send(ToCommand(request), cancellationToken);
        var response = new CreateProgramResponse(ApiProgram.FromApplication(program));

        return CreatedAtAction(nameof(Get), new { id = program.Id }, response);
    }

    /// <summary>Update program</summary>
    /// <param name="id">Program id</param>
    /// <param name="request">Request body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Updated program</response>
    /// <response code="400">Invalid request</response>
    /// <response code="404">Program not found</response>
    /// <returns>Updated program</returns>
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

    /// <summary>Delete program</summary>
    /// <param name="id">Program id</param>
    /// <param name="request">Request body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="204">Program deleted</response>
    /// <response code="400">Invalid request</response>
    /// <response code="404">Program not found</response>
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

    /// <summary>Gets all programs</summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">List of programs</response>
    /// <returns>List of programs</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetProgramsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetProgramsResponse>> GetAll(CancellationToken cancellationToken)
    {
        var programs = await mediator.Send(new GetProgramsQuery(), cancellationToken);
        return Ok(new GetProgramsResponse(programs.Select(ApiProgram.FromApplication).ToArray()));
    }

    /// <summary>Gets program by id</summary>
    /// <param name="id">Program id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Program</response>
    /// <response code="404">Program not found</response>
    /// <returns>Program response object</returns>
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
            request.Title,
            request.Description,
            request.State,
            request.StartDate,
            request.FinishDate,
            request.MinTransactionAmount,
            request.MaxTransactionAmount,
            request.TransactionType,
            new CreateProgramAchievementData(
                request.Achievement.TransactionsCountToApplyAchievement,
                new CreateProgramRewardData(
                    request.Achievement.Reward.Amount,
                    request.Achievement.Reward.Type,
                    request.Achievement.Reward.Target,
                    request.Achievement.Reward.UsageType)),
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
            new CreateProgramAchievementData(
                request.Achievement.TransactionsCountToApplyAchievement,
                new CreateProgramRewardData(
                    request.Achievement.Reward.Amount,
                    request.Achievement.Reward.Type,
                    request.Achievement.Reward.Target,
                    request.Achievement.Reward.UsageType)),
            request.UpdatedBy);
}
