using Frontend.Sync;
using SQLite;

namespace Frontend.Data;

public sealed class LocalTodoService
{
    private readonly SqliteDatabase database;

    public LocalTodoService(SqliteDatabase database)
    {
        this.database = database;
    }

    public async Task<LocalTodoItem> CreateAsync(string title)
    {
        var normalizedTitle = title.Trim();
        if (normalizedTitle.Length is < 1 or > 200)
        {
            throw new ArgumentException("タイトルは1文字以上200文字以内で入力してください。", nameof(title));
        }

        var now = DateTime.UtcNow;
        var todo = new LocalTodoItem
        {
            Id = Guid.NewGuid().ToString(),
            Title = normalizedTitle,
            IsCompleted = false,
            CreatedAt = now,
            UpdatedAt = now,
            Version = 0,
        };
        var operation = new LocalSyncOperation
        {
            Id = Guid.NewGuid().ToString(),
            TodoItemId = todo.Id,
            OperationType = LocalSyncOperation.CreateOperation,
            EnqueuedAt = now,
        };

        var connection = await database.GetConnectionAsync().ConfigureAwait(false);
        await connection.RunInTransactionAsync(transaction =>
        {
            transaction.Insert(todo);
            transaction.Insert(operation);
        }).ConfigureAwait(false);

        return todo;
    }

    public async Task<IReadOnlyList<LocalTodoListItem>> GetAllAsync()
    {
        var connection = await database.GetConnectionAsync().ConfigureAwait(false);
        return await connection.QueryAsync<LocalTodoListItem>(
            """
            SELECT
                todo.id AS Id,
                todo.title AS Title,
                todo.is_completed AS IsCompleted,
                todo.created_at AS CreatedAt,
                todo.updated_at AS UpdatedAt,
                todo.version AS Version,
                CASE WHEN EXISTS (
                    SELECT 1
                    FROM sync_operations operation
                    WHERE operation.todo_item_id = todo.id
                ) THEN 1 ELSE 0 END AS IsPendingSync
            FROM todo_items todo
            WHERE todo.deleted_at IS NULL
            ORDER BY todo.created_at DESC
            """).ConfigureAwait(false);
    }

    public async Task<int> GetPendingCountAsync()
    {
        var connection = await database.GetConnectionAsync().ConfigureAwait(false);
        return await connection.Table<LocalSyncOperation>().CountAsync().ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<PendingCreateOperation>> GetPendingCreatesAsync()
    {
        var connection = await database.GetConnectionAsync().ConfigureAwait(false);
        return await connection.QueryAsync<PendingCreateOperation>(
            """
            SELECT
                operation.id AS OperationId,
                operation.todo_item_id AS TodoItemId,
                todo.title AS Title
            FROM sync_operations operation
            INNER JOIN todo_items todo ON todo.id = operation.todo_item_id
            WHERE operation.operation_type = ?
            ORDER BY operation.enqueued_at, operation.id
            """,
            LocalSyncOperation.CreateOperation).ConfigureAwait(false);
    }

    public async Task CompleteCreateSyncAsync(
        PendingCreateOperation operation,
        TodoApiItem serverItem)
    {
        if (!string.Equals(operation.TodoItemId, serverItem.Id.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("同期応答のTODO IDが送信したIDと一致しません。");
        }

        var connection = await database.GetConnectionAsync().ConfigureAwait(false);
        await connection.RunInTransactionAsync(transaction =>
        {
            var todo = transaction.Find<LocalTodoItem>(operation.TodoItemId)
                ?? throw new InvalidOperationException("同期対象のローカルTODOが見つかりません。");

            todo.Title = serverItem.Title;
            todo.IsCompleted = serverItem.IsCompleted;
            todo.CreatedAt = serverItem.CreatedAt.UtcDateTime;
            todo.UpdatedAt = serverItem.UpdatedAt.UtcDateTime;
            todo.DeletedAt = serverItem.DeletedAt?.UtcDateTime;
            todo.Version = serverItem.Version;

            transaction.Update(todo);
            transaction.Execute(
                "DELETE FROM sync_operations WHERE id = ?",
                operation.OperationId);
        }).ConfigureAwait(false);
    }

    public async Task RecordSyncFailureAsync(string operationId, string error)
    {
        const int MaximumStoredErrorLength = 1000;
        var storedError = error.Length <= MaximumStoredErrorLength
            ? error
            : error[..MaximumStoredErrorLength];

        var connection = await database.GetConnectionAsync().ConfigureAwait(false);
        await connection.ExecuteAsync(
            """
            UPDATE sync_operations
            SET attempt_count = attempt_count + 1,
                last_attempt_at = ?,
                last_error = ?
            WHERE id = ?
            """,
            DateTime.UtcNow,
            storedError,
            operationId).ConfigureAwait(false);
    }
}
