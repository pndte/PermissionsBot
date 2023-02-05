using Telegram.Bot.Types;

namespace PermissionsBot.DB;

public class PlannedActionsDatabase : AbstractDatabase
{
    public PlannedActionsDatabase(string tableName) : base(tableName,
        "userid INTEGER NOT NULL UNIQUE, action TEXT NOT NULL, chatid INTEGER, usermessageid INTEGER, botmessageid INTEGER")
    {
    }

    public void AddUser(long userId, string action)
    {
        if (ContainUser(userId)) RemoveData(userId);
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"INSERT INTO {TableName} (userid, action) VALUES ('{userId}', '{action}')";
            command.ExecuteNonQueryAsync();
        });
    }

    public void AddUser(long userId, string action, int userMessageId, long chatId)
    {
        if (ContainUser(userId)) RemoveData(userId);
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"INSERT INTO {TableName} (userid, action, usermessageid, chatid) VALUES ('{userId}', '{action}', '{userMessageId}', '{chatId}')";
            command.ExecuteNonQueryAsync();
        });
    }

    public void AddUser(long userId, string action, int userMessageId, int botMessageId, long chatId)
    {
        if (ContainUser(userId)) RemoveData(userId);
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"INSERT INTO {TableName} (userid, action, usermessageid, botmessageid, chatid) VALUES ('{userId}', '{action}', '{userMessageId}', '{botMessageId}', '{chatId}')";
            command.ExecuteNonQueryAsync();
        });
    }

    public void RemoveData(long userId)
    {
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"DELETE FROM {TableName} WHERE userid = '{userId}'";
            command.ExecuteNonQuery();
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
                $@"SELECT action FROM {TableName} WHERE userid = '{userId}'";
            result = (string)command.ExecuteScalar();
        });
        return result;
    }

    public int GetUserMessageId(long userId)
    {
        int? result = 0;
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT usermessageid FROM {TableName} WHERE userid = '{userId}'";
            result = Convert.ToInt32(command.ExecuteScalar());
        });
        if (result == null)
        {
            return 0;
        }

        return result.Value;
    }

    public int GetBotMessageId(long userId)
    {
        int? result = 0;
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT botmessageid FROM {TableName} WHERE userid = '{userId}'";
            result = Convert.ToInt32(command.ExecuteScalar());
        });
        if (result == null)
        {
            return 0;
        }

        return result.Value;
    }

    public long GetChatId(long userId)
    {
        long? result = 0;
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT chatid FROM {TableName} WHERE userid = '{userId}'";
            result = (int)command.ExecuteScalar();
        });
        if (result == null)
        {
            return 0;
        }

        return result.Value;
    }
}