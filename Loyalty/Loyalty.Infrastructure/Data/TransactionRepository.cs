using Loyalty.Application.Abstractions;
using Loyalty.Domain.Entities;
using MongoDB.Driver;

namespace Loyalty.Infrastructure.Data;

public sealed class TransactionRepository(IMongoDatabase database) : ITransactionRepository
{
    private readonly IMongoCollection<Documents.TransactionDocument> _transactions =
        database.GetCollection<Documents.TransactionDocument>(CollectionNames.Transactions);

    public async Task<Transaction?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var document = await _transactions.Find(t => t.Id == id).FirstOrDefaultAsync(cancellationToken);
        return document is null ? null : TransactionDocumentMapper.ToDomain(document);
    }

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        try
        {
            await _transactions.InsertOneAsync(
                TransactionDocumentMapper.ToDocument(transaction),
                cancellationToken: cancellationToken);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new InvalidOperationException("Transaction already exists in database.", ex);
        }
    }
}
