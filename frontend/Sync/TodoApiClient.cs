using System.Net.Http.Json;

namespace Frontend.Sync;

public sealed class TodoApiClient
{
    private readonly HttpClient httpClient;

    public TodoApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<TodoApiItem> CreateAsync(
        Guid id,
        string title,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "todos",
            new CreateTodoApiRequest { Id = id, Title = title },
            cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);
            throw new HttpRequestException(
                $"TODO API returned {(int)response.StatusCode} ({response.ReasonPhrase}). {responseBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<TodoApiItem>(cancellationToken)
                   .ConfigureAwait(false)
               ?? throw new InvalidOperationException("TODO APIから空の応答が返されました。");
    }
}
