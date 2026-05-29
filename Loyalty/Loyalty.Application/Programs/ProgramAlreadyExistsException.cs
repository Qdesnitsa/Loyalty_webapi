using Loyalty.Application.Common;

namespace Loyalty.Application.Programs;

public sealed class ProgramAlreadyExistsException(string programId)
    : LoyaltyException($"Program '{programId}' already exists.")
{
    public string ProgramId { get; } = programId;
}
