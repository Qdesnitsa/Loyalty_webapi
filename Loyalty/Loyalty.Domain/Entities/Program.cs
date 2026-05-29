namespace Loyalty.Domain.Entities;

public class Program
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public ProgramState State { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime FinishDate { get; init; }
    public decimal MinTransactionAmount { get; init; }
    public decimal MaxTransactionAmount { get; init; }
    public required Achievement Achievement { get; init; }
    public DateTime CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
    public required string UpdatedBy { get; init; }

    public static Program Create(
        string id,
        string title,
        string? description,
        ProgramState state,
        DateTime startDate,
        DateTime finishDate,
        decimal minTransactionAmount,
        decimal maxTransactionAmount,
        Achievement achievement,
        string createdBy)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Program id is required.", nameof(id));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Program title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("CreatedBy is required.", nameof(createdBy));

        if (finishDate <= startDate)
            throw new ArgumentException("Finish date must be after start date.", nameof(finishDate));

        if (minTransactionAmount > maxTransactionAmount)
            throw new ArgumentException("Min transaction amount cannot exceed max transaction amount.");

        var now = DateTime.UtcNow;

        return new Program
        {
            Id = id.Trim(),
            Title = title.Trim(),
            Description = description?.Trim(),
            State = state,
            StartDate = startDate,
            FinishDate = finishDate,
            MinTransactionAmount = minTransactionAmount,
            MaxTransactionAmount = maxTransactionAmount,
            Achievement = achievement,
            CreatedAt = now,
            CreatedBy = createdBy.Trim(),
            UpdatedAt = now,
            UpdatedBy = createdBy.Trim()
        };
    }
}

public enum ProgramState
{
    Active,
    NotActive
}
