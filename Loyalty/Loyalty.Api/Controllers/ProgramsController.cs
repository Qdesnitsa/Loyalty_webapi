using Loyalty.Api.Contracts.Programs;

using Loyalty.Application.Abstractions;

using Loyalty.Application.Programs;

using Loyalty.Domain.Entities;

using Microsoft.AspNetCore.Mvc;



namespace Loyalty.Api.Controllers;



[ApiController]

[Route("api/programs")]

public sealed class ProgramsController(

    CreateProgramService createProgramService,

    IProgramRepository programRepository) : ControllerBase

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

        var program = await createProgramService.CreateAsync(ToCommand(request), cancellationToken);

        var response = new CreateProgramResponse(ProgramResponse.FromDomain(program));



        return CreatedAtAction(nameof(Get), new { id = program.Id }, response);

    }



    [HttpGet("{id}")]

    [ProducesResponseType(typeof(GetProgramResponse), StatusCodes.Status200OK)]

    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

    public async Task<ActionResult<GetProgramResponse>> Get(string id, CancellationToken cancellationToken)

    {

        var program = await programRepository.GetByIdAsync(id, cancellationToken);

        if (program is null)

            return NotFound(new ProblemDetails

            {

                Status = StatusCodes.Status404NotFound,

                Title = "Program not found",

                Detail = $"Program '{id}' was not found.",

                Instance = HttpContext.Request.Path

            });



        return Ok(new GetProgramResponse(ProgramResponse.FromDomain(program)));

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

            new Achievement

            {

                Id = request.Achievement.Id,

                TransactionsCountToApplyAchievement = request.Achievement.TransactionsCountToApplyAchievement,

                Reward = new Reward

                {

                    Amount = request.Achievement.Reward.Amount,

                    Type = request.Achievement.Reward.Type,

                    Target = request.Achievement.Reward.Target,

                    UsageType = request.Achievement.Reward.UsageType

                }

            },

            request.CreatedBy);

}


