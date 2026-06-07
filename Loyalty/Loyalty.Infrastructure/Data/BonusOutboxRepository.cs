using Loyalty.Application.Abstractions;
using Loyalty.Application.BonusOutbox;
using Loyalty.Infrastructure.Data.Documents;
using MongoDB.Driver;

namespace Loyalty.Infrastructure.Data;

public sealed class BonusOutboxRepository(IMongoDatabase database) : IBonusOutboxRepository
{
    private readonly IMongoCollection<BonusOutboxDocument> _outbox =
        database.GetCollection<BonusOutboxDocument>(CollectionNames.BonusOutbox);

    public async Task<bool> TryAddAsync(BonusOutboxEntry entry, CancellationToken cancellationToken = default)
    {
        try
        {
            await _outbox.InsertOneAsync(
                BonusOutboxDocumentMapper.ToDocument(entry),
                cancellationToken: cancellationToken);
            return true;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return false;
        }
    }

    public async Task<IReadOnlyList<BonusOutboxEntry>> GetPendingAsync(
        int take,
        CancellationToken cancellationToken = default)
    {
        var documents = await _outbox
            .Find(FilterDefinition<BonusOutboxDocument>.Empty)
            .SortBy(x => x.OccurredAt)
            .Limit(take)
            .ToListAsync(cancellationToken);

        return documents.Select(BonusOutboxDocumentMapper.ToDomain).ToArray();
    }

    public async Task DeleteAsync(string sourceTransactionId, CancellationToken cancellationToken = default) =>
        await _outbox.DeleteOneAsync(x => x.SourceTransactionId == sourceTransactionId, cancellationToken);
}
