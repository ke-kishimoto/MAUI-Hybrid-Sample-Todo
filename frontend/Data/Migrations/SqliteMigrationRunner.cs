using SQLite;

namespace Frontend.Data.Migrations;

public sealed class SqliteMigrationRunner
{
    private static readonly IReadOnlyList<ISqliteMigration> Migrations =
    [
        new CreateTodoStorageMigration(),
    ];

    public async Task MigrateAsync(SQLiteAsyncConnection database)
    {
        await database.CreateTableAsync<AppliedSqliteMigration>().ConfigureAwait(false);

        var appliedVersions = (await database.Table<AppliedSqliteMigration>()
                .ToListAsync()
                .ConfigureAwait(false))
            .Select(migration => migration.Version)
            .ToHashSet();

        foreach (var migration in Migrations.OrderBy(migration => migration.Version))
        {
            if (appliedVersions.Contains(migration.Version))
            {
                continue;
            }

            await database.RunInTransactionAsync(connection =>
            {
                migration.Up(connection);
                connection.Insert(new AppliedSqliteMigration
                {
                    Version = migration.Version,
                    Name = migration.Name,
                    AppliedAt = DateTime.UtcNow,
                });
            }).ConfigureAwait(false);
        }
    }
}
