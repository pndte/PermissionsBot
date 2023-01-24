using Microsoft.Data.Sqlite;

namespace PermissionsBot.DB;

public abstract class AbstractDatabase
{
    public AbstractDatabase(string tableName, string tableColumns)
    {
        TableName = tableName;
        ConnectAndRun((connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                @$"CREATE TABLE IF NOT EXISTS {TableName}                 
                        ({tableColumns});";
            command.ExecuteNonQuery();
        }));
    }


    protected string TableName;

    protected delegate void DatabaseProcedure(SqliteConnection connection);

    protected void ConnectAndRun(DatabaseProcedure databaseProcedure)
    {
        using (var connection = new SqliteConnection($"Data Source=data.db; Mode=ReadWriteCreate;"))
        {
            connection.Open();
            databaseProcedure(connection);
        }
    }
}