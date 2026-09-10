using Microsoft.Maui.Storage;
using SQLite;
using Frontend.Data.Migrations;

namespace Frontend.Data;

public sealed class SqliteDatabase
{
    private const string DatabaseFileName = "todo.db3";

    private readonly string databasePath;
    private readonly SemaphoreSlim initializationLock = new(1, 1);
    private readonly SqliteMigrationRunner migrationRunner = new();
    private SQLiteAsyncConnection? connection;

    public SqliteDatabase()
        : this(Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName))
    {
    }

    internal SqliteDatabase(string databasePath)
    {
        this.databasePath = databasePath;
    }

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
                var databaseDirectory = Path.GetDirectoryName(databasePath);
                if (!string.IsNullOrEmpty(databaseDirectory))
                {
                    Directory.CreateDirectory(databaseDirectory);
                }

                var newConnection = new SQLiteAsyncConnection(
                    databasePath,
                    SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

                await newConnection.ExecuteAsync("PRAGMA foreign_keys = ON").ConfigureAwait(false);
                await migrationRunner.MigrateAsync(newConnection).ConfigureAwait(false);
                connection = newConnection;
            }

            return connection;
        }
        finally
        {
            initializationLock.Release();
        }
    }
}
