namespace Loyalty.Api.Contracts.Programs;

/// <summary>Delete program request</summary>
/// <param name="UpdatedBy">User who deleted the program</param>
public sealed record DeleteProgramRequest(string UpdatedBy);
