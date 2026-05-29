using Loyalty.Application.Abstractions;
using Loyalty.Domain.Entities;
using Program = Loyalty.Domain.Entities.Program;

using Loyalty.Infrastructure.Data.Documents;
using MongoDB.Driver;

namespace Loyalty.Infrastructure.Data;

public sealed class ProgramRepository(IMongoDatabase database) : IProgramRepository
{
    private readonly IMongoCollection<ProgramDocument> _programs =
        database.GetCollection<ProgramDocument>(CollectionNames.Programs);

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default) =>
        await _programs.Find(p => p.Id == id).AnyAsync(cancellationToken);

    public async Task<Program?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var document = await _programs.Find(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
        return document is null ? null : ProgramDocumentMapper.ToDomain(document);
    }

    public async Task AddAsync(Program program, CancellationToken cancellationToken = default) =>
        await _programs.InsertOneAsync(ProgramDocumentMapper.ToDocument(program), cancellationToken: cancellationToken);
}
