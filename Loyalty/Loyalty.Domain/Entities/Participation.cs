using System.Globalization;

namespace Loyalty.Domain.Entities;

public sealed class Participation
{
    public required string Id { get; init; }
    public int CardId { get; init; }
    public required string ProgramId { get; init; }
    public int QualifyingTransactionCount { get; init; }
    public DateTime UpdatedAt { get; init; }

    public static string BuildId(int cardId, string programId) =>
        $"{cardId.ToString(CultureInfo.InvariantCulture)}:{programId}";
}
