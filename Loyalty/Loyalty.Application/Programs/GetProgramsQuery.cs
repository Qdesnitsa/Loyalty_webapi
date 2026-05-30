using Loyalty.Application.Abstractions;
using Loyalty.Application.Programs.Models;

namespace Loyalty.Application.Programs;

public sealed record GetProgramsQuery : IQuery<IReadOnlyList<Program>>;

public sealed class GetProgramsQueryHandler(IProgramRepository programRepository)
    : IQueryHandler<GetProgramsQuery, IReadOnlyList<Program>>
{
    public async Task<IReadOnlyList<Program>> Handle(GetProgramsQuery query, CancellationToken cancellationToken)
    {
        var domainPrograms = await programRepository.GetAllNotDeletedAsync(cancellationToken);
        return domainPrograms.Select(Program.FromDomain).ToList();
    }
}
