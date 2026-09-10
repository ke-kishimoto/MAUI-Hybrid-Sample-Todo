using Frontend.Data;
using Xunit;

namespace Frontend.Tests.Data;

public sealed class LocalTodoServiceTests
{
    [Fact]
    public async Task CreateAsync_SavesTodoAndCreateOperationInOneLocalDatabase()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"todo-test-{Guid.NewGuid():N}.db3");
        var database = new SqliteDatabase(databasePath);

        try
        {
            var service = new LocalTodoService(database);

            var created = await service.CreateAsync("  牛乳を買う  ");

            var todo = Assert.Single(await service.GetAllAsync());
            var operation = Assert.Single(await service.GetPendingCreatesAsync());
            Assert.Equal("牛乳を買う", created.Title);
            Assert.Equal(created.Id, todo.Id);
            Assert.True(todo.IsPendingSync);
            Assert.Equal(created.Id, operation.TodoItemId);
            Assert.Equal(1, await service.GetPendingCountAsync());
        }
        finally
        {
            await (await database.GetConnectionAsync()).CloseAsync();
            File.Delete(databasePath);
        }
    }
}
