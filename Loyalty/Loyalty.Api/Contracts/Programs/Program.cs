using Loyalty.Domain.Entities;
using ApplicationProgram = Loyalty.Application.Programs.Models.Program;

namespace Loyalty.Api.Contracts.Programs;

/// <summary>Loyalty program</summary>
/// <param name="Id">Program id</param>
/// <param name="Title">Program title</param>
/// <param name="Description">Program description</param>
/// <param name="State">Program state</param>
/// <param name="StartDate">Program start date</param>
/// <param name="FinishDate">Program finish date</param>
/// <param name="MinTransactionAmount">Minimum transaction amount to apply the program</param>
/// <param name="MaxTransactionAmount">Maximum transaction amount to apply the program</param>
/// <param name="TransactionType">Applicable transaction type</param>
/// <param name="Achievement">Program achievement</param>
/// <param name="CreatedAt">Created timestamp (UTC)</param>
/// <param name="CreatedBy">User who created the program</param>
/// <param name="UpdatedAt">Updated timestamp (UTC)</param>
/// <param name="UpdatedBy">User who last updated the program</param>
/// <param name="IsDeleted">Whether the program is soft-deleted</param>
public sealed record Program(
    string Id,
    string Title,
    string? Description,
    ProgramState State,
    DateTime StartDate,
    DateTime FinishDate,
    decimal MinTransactionAmount,
    decimal MaxTransactionAmount,
    string TransactionType,
    Achievement Achievement,
    DateTime CreatedAt,
    string CreatedBy,
    DateTime UpdatedAt,
    string UpdatedBy,
    bool IsDeleted)
{
    public static Program FromApplication(ApplicationProgram program) =>
        new(
            program.Id,
            program.Title,
            program.Description,
            program.State,
            program.StartDate,
            program.FinishDate,
            program.MinTransactionAmount,
            program.MaxTransactionAmount,
            program.TransactionType,
            Achievement.FromApplication(program.Achievement),
            program.CreatedAt,
            program.CreatedBy,
            program.UpdatedAt,
            program.UpdatedBy,
            program.IsDeleted);
}

/// <summary>Program achievement</summary>
/// <param name="Id">Achievement id</param>
/// <param name="TransactionsCountToApplyAchievement">Number of qualifying transactions required before reward is granted</param>
/// <param name="Reward">Achievement reward</param>
public sealed record Achievement(
    string Id,
    int TransactionsCountToApplyAchievement,
    Reward Reward)
{
    public static Achievement FromApplication(Application.Programs.Models.Achievement achievement) =>
        new(
            achievement.Id,
            achievement.TransactionsCountToApplyAchievement,
            Reward.FromApplication(achievement.Reward));
}

/// <summary>Program reward</summary>
/// <param name="Id">Reward id</param>
/// <param name="Amount">Reward amount</param>
/// <param name="Type">Reward value type</param>
/// <param name="Target">Reward target</param>
/// <param name="UsageType">How the reward value is applied</param>
public sealed record Reward(
    string Id,
    decimal Amount,
    RewardValueType Type,
    RewardTarget Target,
    RewardValueUsageType UsageType)
{
    public static Reward FromApplication(Application.Programs.Models.Reward reward) =>
        new(reward.Id, reward.Amount, reward.Type, reward.Target, reward.UsageType);
}

/// <summary>Create program response</summary>
/// <param name="Program">Created program</param>
public sealed record CreateProgramResponse(Program Program);

/// <summary>Update program response</summary>
/// <param name="Program">Updated program</param>
public sealed record UpdateProgramResponse(Program Program);

/// <summary>Get program response</summary>
/// <param name="Program">Program</param>
public sealed record GetProgramResponse(Program Program);

/// <summary>Get programs response</summary>
/// <param name="Programs">List of programs</param>
public sealed record GetProgramsResponse(Program[] Programs);
