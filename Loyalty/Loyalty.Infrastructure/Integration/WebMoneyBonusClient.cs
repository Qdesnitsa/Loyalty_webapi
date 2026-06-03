using System.Net.Http.Json;
using Loyalty.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace Loyalty.Infrastructure.Integration;

public sealed class WebMoneyBonusClient(
    HttpClient httpClient,
    IOptions<WebMoneyOptions> options) : IWebMoneyBonusClient
{
    public async Task AccrueBonusAsync(
        int cardId,
        decimal amount,
        string sourceTransactionId,
        string? programId,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"api/loyalty/cards/{cardId}/bonus");

        request.Headers.Add("X-Loyalty-Api-Key", options.Value.ApiKey);
        request.Content = JsonContent.Create(new AccrueBonusRequestBody(amount, sourceTransactionId, programId));

        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException($"Card {cardId} was not found in WebMoney.");
        }

        response.EnsureSuccessStatusCode();
    }

    private sealed record AccrueBonusRequestBody(
        decimal Amount,
        string SourceTransactionId,
        string? ProgramId);
}
