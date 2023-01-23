using Microsoft.Data.Sqlite;

namespace PermissionsBot.DB;

public abstract class AbstractDatabase
{
    protected delegate void DatabaseProcedure(SqliteConnection connection);

    protected void ConnectAndRun(DatabaseProcedure databaseProcedure)
    {
        using (var connection = new SqliteConnection($"Data Source=data.db; Mode=ReadWriteCreate;"))
        {
            connection.Open();
            databaseProcedure.Invoke(connection);
        }
    } 
}