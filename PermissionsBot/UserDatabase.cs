using Microsoft.Data.Sqlite;

namespace PermissionsBot.DB;

public class UserDatabase
{
    public UserDatabase(string databaseName)
    {
        _databaseName = databaseName;
    }

    private string _databaseName;

    public Command? GetPermissions(long userId)
    {
        return null;
    }

    public void AddToken(string token)
    {
        ConnectAndRun((SqliteConnection connection) =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                @"INSERT INTO userdata (token, id, permissions)
                  VALUES ($token, 0, 0)";
            //command.Parameters.AddWithValue("$tableName", _databaseName);
            command.Parameters.AddWithValue("$token", token);
            Console.WriteLine("Done");
        });
    }

    private delegate void DatabaseProcedure(SqliteConnection connection);

    private void ConnectAndRun(DatabaseProcedure databaseProcedure)
    {
        using (var connection = new SqliteConnection("Data Source=userdata.db; Mode=ReadWriteCreate;"))
        {
            connection.Open();
            databaseProcedure.Invoke(connection);
            connection.Close();
        }
    }
}