using SQLite;

namespace Frontend.Data;

[Table("todo_items")]
public class LocalTodoItem
{
    [PrimaryKey]
    [Column("id")]
    public string Id { get; set; } = string.Empty;

    [NotNull]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [NotNull]
    [Column("is_completed")]
    public bool IsCompleted { get; set; }

    [NotNull]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [NotNull]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [NotNull]
    [Column("version")]
    public long Version { get; set; }
}
