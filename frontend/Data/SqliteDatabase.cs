using Microsoft.Maui.Storage;
using SQLite;

namespace Frontend.Data;

public sealed class SqliteDatabase
{
    private const string DatabaseFileName = "todo.db3";

    private readonly SemaphoreSlim initializationLock = new(1, 1);
    private SQLiteAsyncConnection? connection;

    public async Task InitializeAsync()
    {
        _ = await GetConnectionAsync();
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (connection is not null)
        {
            return connection;
        }

        await initializationLock.WaitAsync();
        try
        {
            if (connection is null)
            {
                var databasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
                Directory.CreateDirectory(FileSystem.AppDataDirectory);
                connection = new SQLiteAsyncConnection(databasePath);
                await connection.CreateTableAsync<LocalTodoItem>().ConfigureAwait(false);
            }

            return connection;
        }
        finally
        {
            initializationLock.Release();
        }
    }
}
