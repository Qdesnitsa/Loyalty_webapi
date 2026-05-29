using Program = Loyalty.Domain.Entities.Program;

namespace Loyalty.Application.Abstractions;

public interface IProgramRepository
{
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);

    Task<Program?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task AddAsync(Program program, CancellationToken cancellationToken = default);
}
