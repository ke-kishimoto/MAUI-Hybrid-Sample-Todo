using SQLite;

namespace Frontend.Data.Migrations;

public interface ISqliteMigration
{
    long Version { get; }

    string Name { get; }

    void Up(SQLiteConnection database);
}
