using System.Net;
using System.Net.Http.Json;
using Frontend.Data;
using Frontend.Sync;
using Xunit;

namespace Frontend.Tests.Sync;

public sealed class TodoSyncServiceTests
{
    [Fact]
    public async Task SyncAsync_OnCreatedResponse_UpdatesLocalTodoAndRemovesOperation()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"todo-test-{Guid.NewGuid():N}.db3");
        var database = new SqliteDatabase(databasePath);

        try
        {
            var localTodos = new LocalTodoService(database);
            var localTodo = await localTodos.CreateAsync("同期するTODO");
            using var httpClient = new HttpClient(new StubHttpMessageHandler(async request =>
            {
                Assert.Equal(HttpMethod.Post, request.Method);
                Assert.Equal("http://localhost:8080/api/v1/todos", request.RequestUri?.ToString());
                var posted = await request.Content!.ReadFromJsonAsync<CreateTodoApiRequest>();
                Assert.Equal(localTodo.Id, posted!.Id.ToString());

                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = JsonContent.Create(new TodoApiItem
                    {
                        Id = posted.Id,
                        Title = posted.Title,
                        IsCompleted = false,
                        CreatedAt = new DateTimeOffset(2026, 9, 10, 0, 0, 0, TimeSpan.Zero),
                        UpdatedAt = new DateTimeOffset(2026, 9, 10, 0, 0, 1, TimeSpan.Zero),
                        Version = 3,
                    }),
                };
            }))
            {
                BaseAddress = new Uri("http://localhost:8080/api/v1/"),
            };
            var service = new TodoSyncService(localTodos, new TodoApiClient(httpClient));

            var result = await service.SyncAsync();

            Assert.Equal(1, result.SucceededCount);
            Assert.Equal(0, result.FailedCount);
            Assert.Equal(0, await localTodos.GetPendingCountAsync());
            var todo = Assert.Single(await localTodos.GetAllAsync());
            Assert.False(todo.IsPendingSync);
            Assert.Equal(3, todo.Version);
        }
        finally
        {
            await (await database.GetConnectionAsync()).CloseAsync();
            File.Delete(databasePath);
        }
    }

    [Fact]
    public async Task SyncAsync_OnApiFailure_KeepsOperationForRetry()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"todo-test-{Guid.NewGuid():N}.db3");
        var database = new SqliteDatabase(databasePath);

        try
        {
            var localTodos = new LocalTodoService(database);
            await localTodos.CreateAsync("あとで同期するTODO");
            using var httpClient = new HttpClient(new StubHttpMessageHandler(_ => Task.FromResult(
                new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent("backend unavailable"),
                })))
            {
                BaseAddress = new Uri("http://localhost:8080/api/v1/"),
            };
            var service = new TodoSyncService(localTodos, new TodoApiClient(httpClient));

            var result = await service.SyncAsync();

            Assert.Equal(0, result.SucceededCount);
            Assert.Equal(1, result.FailedCount);
            Assert.Equal(1, await localTodos.GetPendingCountAsync());

            var connection = await database.GetConnectionAsync();
            var operation = Assert.Single(await connection.Table<LocalSyncOperation>().ToListAsync());
            Assert.Equal(1, operation.AttemptCount);
            Assert.NotNull(operation.LastAttemptAt);
            Assert.Contains("503", operation.LastError);
        }
        finally
        {
            await (await database.GetConnectionAsync()).CloseAsync();
            File.Delete(databasePath);
        }
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> responseFactory;

        public StubHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> responseFactory)
        {
            this.responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return responseFactory(request);
        }
    }
}
