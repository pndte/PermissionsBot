using System.Text;

namespace PermissionsBot.DB;

public class ChatDatabase : AbstractDatabase
{
    public ChatDatabase(string tableName) : base(tableName, "chatid INTEGER NOT NULL UNIQUE, grade INTEGER NOT NULL, chatsettings INTEGER NOT NULL")
    {
    }

    public bool ContainChat(long chatId)
    {
        bool result = false;
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT chatid FROM {TableName} WHERE chatid = '{chatId}'";
            var id = command.ExecuteScalar();
            if (id != null)
            {
                result = true;
            }
        });
        return result;
    }

    public List<long> GetAllChats()
    {
        List<long> ids = new List<long>();
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT * FROM {TableName}";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    ids.Add(long.Parse(reader.GetString(0)));
                }
            }
        });
        return ids;
    }

    public List<long> GetAllChatsByGrade(byte grade)
    {
        List<long> ids = new List<long>();
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT * FROM {TableName} WHERE grade = '{grade}'";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    ids.Add(long.Parse(reader.GetString(0)));
                }
            }
        });
        return ids;
    }

    public List<long> GetAllChatsByGrades(byte[] grades)
    {
        List<long> ids = new List<long>();
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            foreach (var grade in grades)
            {
                command.CommandText =
                    $@"SELECT * FROM {TableName} WHERE grade = '{grade}'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ids.Add(long.Parse(reader.GetString(0)));
                    }
                }
            }
        });
        return ids;
    }

    public void AddChat(long chatId, byte grade)
    {
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"INSERT INTO {TableName} (chatid, grade, chatsettings)
                   VALUES ('{chatId}', '{grade}', '0');";
            command.ExecuteNonQuery();
        });
    }

    public void RemoveData(long chatId)
    {
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"DELETE FROM {TableName} WHERE chatid = '{chatId}'";
            command.ExecuteNonQuery();
        });
    }
}