using Frontend.Data;
using Frontend.Data.Migrations;
using SQLite;
using Xunit;

namespace Frontend.Tests.Data;

public sealed class SqliteMigrationRunnerTests
{
    [Fact]
    public async Task MigrateAsync_CreatesTodoOutboxAndHistoryTables_Idempotently()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"todo-test-{Guid.NewGuid():N}.db3");
        var database = new SQLiteAsyncConnection(databasePath);

        try
        {
            // Reproduce the schema made by the previous CreateTableAsync-based implementation.
            await database.CreateTableAsync<LocalTodoItem>();

            var runner = new SqliteMigrationRunner();
            await runner.MigrateAsync(database);
            await runner.MigrateAsync(database);

            var tables = await database.QueryAsync<SqliteObject>(
                "SELECT name FROM sqlite_master WHERE type = 'table'");
            Assert.Contains(tables, table => table.Name == "todo_items");
            Assert.Contains(tables, table => table.Name == "sync_operations");
            Assert.Contains(tables, table => table.Name == "__schema_migrations");

            var appliedMigrations = await database.Table<AppliedSqliteMigration>().ToListAsync();
            var appliedMigration = Assert.Single(appliedMigrations);
            Assert.Equal(1, appliedMigration.Version);

            var syncColumns = await database.QueryAsync<TableColumn>(
                "PRAGMA table_info(sync_operations)");
            Assert.Contains(syncColumns, column => column.Name == "todo_item_id");
            Assert.Contains(syncColumns, column => column.Name == "operation_type");
            Assert.Contains(syncColumns, column => column.Name == "attempt_count");
        }
        finally
        {
            await database.CloseAsync();
            File.Delete(databasePath);
        }
    }

    private sealed class SqliteObject
    {
        [Column("name")]
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TableColumn
    {
        [Column("name")]
        public string Name { get; set; } = string.Empty;
    }
}
