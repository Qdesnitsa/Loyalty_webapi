using Loyalty.Application.Abstractions;
using Loyalty.Application.Programs.Models;

namespace Loyalty.Application.Programs;

public sealed record GetProgramQuery(string Id) : IQuery<Program>;

public sealed class GetProgramQueryHandler(IProgramRepository programRepository)
    : IQueryHandler<GetProgramQuery, Program>
{
    public async Task<Program> Handle(GetProgramQuery query, CancellationToken cancellationToken)
    {
        var domainProgram = await programRepository.GetByIdAsync(query.Id, cancellationToken);
        if (domainProgram is null)
        {
            throw new InvalidQueryException("Program not found", ExceptionTypes.ResourceNotFound);
        }
        return Program.FromDomain(domainProgram);
    }
}
