using SQLite;

namespace Frontend.Data;

[Table("sync_operations")]
public sealed class LocalSyncOperation
{
    public const string CreateOperation = "create";
    public const string UpdateOperation = "update";
    public const string DeleteOperation = "delete";

    [PrimaryKey]
    [Column("id")]
    public string Id { get; set; } = string.Empty;

    [NotNull]
    [Indexed]
    [Column("todo_item_id")]
    public string TodoItemId { get; set; } = string.Empty;

    [NotNull]
    [Column("operation_type")]
    public string OperationType { get; set; } = string.Empty;

    [Column("base_version")]
    public long? BaseVersion { get; set; }

    [NotNull]
    [Column("enqueued_at")]
    public DateTime EnqueuedAt { get; set; }

    [NotNull]
    [Column("attempt_count")]
    public int AttemptCount { get; set; }

    [Column("last_attempt_at")]
    public DateTime? LastAttemptAt { get; set; }

    [Column("last_error")]
    public string? LastError { get; set; }
}
