namespace PermissionsBot.DB;

public class PlannedActionsDatabase: AbstractDatabase
{
    public PlannedActionsDatabase(string tableName) : base(tableName,  "userid INTEGER NOT NULL UNIQUE, action TEXT NOT NULL")
    {
    }

    public void AddUser(long userId, string action)
    {
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"INSERT INTO {TableName} (userid, action) VALUES ('{userId}', '{action}')";
            command.ExecuteNonQueryAsync();
        });
    }

    public bool ContainUser(long userId)
    {
        bool result = false;
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT userid FROM {TableName} WHERE userid = '{userId}'";
            if (command.ExecuteScalar() != null)
            {
                result = true;
            }
        });
        return result;
    }

    public string GetAction(long userId)
    {
        string result = "";
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT userid FROM {TableName} WHERE userid = '{userId}'";
            result =  (string)command.ExecuteScalar();
        });
        return result;
    }
}