using PermissionsBot.DB;

namespace PermissionsBot;

public class ChatDatabase: AbstractDatabase
{
    public ChatDatabase(string tableName)
    {
        _tableName = tableName;
        ConnectAndRun((connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                @$"CREATE TABLE IF NOT EXISTS {tableName}                 
                   (token TEXT NOT NULL UNIQUE, userid INTEGER, permissions INTEGER);";
            command.ExecuteNonQuery();
        }));
    }

    private string _tableName;
}