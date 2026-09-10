using SQLite;

namespace Frontend.Data.Migrations;

[Table("__schema_migrations")]
public sealed class AppliedSqliteMigration
{
    [PrimaryKey]
    [Column("version")]
    public long Version { get; set; }

    [NotNull]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [NotNull]
    [Column("applied_at")]
    public DateTime AppliedAt { get; set; }
}
