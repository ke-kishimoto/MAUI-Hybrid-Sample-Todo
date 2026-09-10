using SQLite;

namespace Frontend.Data.Migrations;

public sealed class CreateTodoStorageMigration : ISqliteMigration
{
    public long Version => 1;

    public string Name => "Create TODO and sync operation tables";

    public void Up(SQLiteConnection database)
    {
        database.Execute(
            """
            CREATE TABLE IF NOT EXISTS todo_items (
                id TEXT NOT NULL PRIMARY KEY,
                title TEXT NOT NULL CHECK (length(trim(title)) BETWEEN 1 AND 200),
                is_completed INTEGER NOT NULL DEFAULT 0 CHECK (is_completed IN (0, 1)),
                created_at INTEGER NOT NULL,
                updated_at INTEGER NOT NULL,
                deleted_at INTEGER NULL,
                version INTEGER NOT NULL DEFAULT 0 CHECK (version >= 0)
            )
            """);

        database.Execute(
            """
            CREATE INDEX IF NOT EXISTS idx_todo_items_updated_at
                ON todo_items (updated_at)
            """);

        database.Execute(
            """
            CREATE TABLE IF NOT EXISTS sync_operations (
                id TEXT NOT NULL PRIMARY KEY,
                todo_item_id TEXT NOT NULL,
                operation_type TEXT NOT NULL
                    CHECK (operation_type IN ('create', 'update', 'delete')),
                base_version INTEGER NULL CHECK (base_version IS NULL OR base_version >= 0),
                enqueued_at INTEGER NOT NULL,
                attempt_count INTEGER NOT NULL DEFAULT 0 CHECK (attempt_count >= 0),
                last_attempt_at INTEGER NULL,
                last_error TEXT NULL,
                FOREIGN KEY (todo_item_id) REFERENCES todo_items (id)
            )
            """);

        database.Execute(
            """
            CREATE UNIQUE INDEX IF NOT EXISTS idx_sync_operations_todo_operation
                ON sync_operations (todo_item_id, operation_type)
            """);

        database.Execute(
            """
            CREATE INDEX IF NOT EXISTS idx_sync_operations_enqueued_at
                ON sync_operations (enqueued_at)
            """);
    }
}
