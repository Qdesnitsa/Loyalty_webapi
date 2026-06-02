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
    public required string TransactionType { get; init; }
    public required Achievement Achievement { get; init; }
    public DateTime CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
    public required string UpdatedBy { get; init; }
    public bool IsDeleted { get; init; }

    public static Program Create(
        string id,
        string title,
        string? description,
        ProgramState state,
        DateTime startDate,
        DateTime finishDate,
        decimal minTransactionAmount,
        decimal maxTransactionAmount,
        string transactionType,
        Achievement achievement,
        string createdBy)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Program id is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Program title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new ArgumentException("CreatedBy is required.", nameof(createdBy));
        }

        if (finishDate <= startDate)
        {
            throw new ArgumentException("Finish date must be after start date.", nameof(finishDate));
        }

        if (minTransactionAmount > maxTransactionAmount)
        {
            throw new ArgumentException("Min transaction amount cannot exceed max transaction amount.");
        }

        if (string.IsNullOrWhiteSpace(transactionType))
        {
            throw new ArgumentException("Transaction type is required.", nameof(transactionType));
        }

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
            TransactionType = transactionType.Trim(),
            Achievement = achievement,
            CreatedAt = now,
            CreatedBy = createdBy.Trim(),
            UpdatedAt = now,
            UpdatedBy = createdBy.Trim(),
            IsDeleted = false
        };
    }

    public Program Update(
        string title,
        string? description,
        ProgramState state,
        DateTime startDate,
        DateTime finishDate,
        decimal minTransactionAmount,
        decimal maxTransactionAmount,
        string transactionType,
        Achievement achievement,
        string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Program title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(updatedBy))
        {
            throw new ArgumentException("UpdatedBy is required.", nameof(updatedBy));
        }

        if (finishDate <= startDate)
        {
            throw new ArgumentException("Finish date must be after start date.", nameof(finishDate));
        }

        if (minTransactionAmount > maxTransactionAmount)
        {
            throw new ArgumentException("Min transaction amount cannot exceed max transaction amount.");
        }

        if (string.IsNullOrWhiteSpace(transactionType))
        {
            throw new ArgumentException("Transaction type is required.", nameof(transactionType));
        }

        var now = DateTime.UtcNow;

        return new Program
        {
            Id = Id,
            Title = title.Trim(),
            Description = description?.Trim(),
            State = state,
            StartDate = startDate,
            FinishDate = finishDate,
            MinTransactionAmount = minTransactionAmount,
            MaxTransactionAmount = maxTransactionAmount,
            TransactionType = transactionType.Trim(),
            Achievement = achievement,
            CreatedAt = CreatedAt,
            CreatedBy = CreatedBy,
            UpdatedAt = now,
            UpdatedBy = updatedBy.Trim(),
            IsDeleted = IsDeleted
        };
    }

    public Program Delete(string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
        {
            throw new ArgumentException("UpdatedBy is required.", nameof(updatedBy));
        }

        var now = DateTime.UtcNow;

        return new Program
        {
            Id = Id,
            Title = Title,
            Description = Description,
            State = State,
            StartDate = StartDate,
            FinishDate = FinishDate,
            MinTransactionAmount = MinTransactionAmount,
            MaxTransactionAmount = MaxTransactionAmount,
            TransactionType = TransactionType,
            Achievement = Achievement,
            CreatedAt = CreatedAt,
            CreatedBy = CreatedBy,
            UpdatedAt = now,
            UpdatedBy = updatedBy.Trim(),
            IsDeleted = true
        };
    }
}

public enum ProgramState
{
    Active,
    NotActive
}
