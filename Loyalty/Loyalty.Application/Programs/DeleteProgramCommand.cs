namespace Loyalty.Application.Programs;

public sealed record DeleteProgramCommand(string Id, string UpdatedBy) : ICommand;
