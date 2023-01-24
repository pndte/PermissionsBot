namespace PermissionsBot.DB;

public class ChatDatabase : AbstractDatabase
{
    public ChatDatabase(string tableName) : base(tableName, "chatid INTEGER NOT NULL UNIQUE, grade INTEGER NOT NULL")
    {
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

    public List<long> GetAllAChatsByGrade(byte grade)
    {
        List<long> ids = new List<long>();
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT * FROM {TableName} WHERE grade = '{grade}' ORDER BY grade ASC";
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

    public void AddChat(long chatId, byte grade)
    {
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"INSERT INTO {TableName} (chatid, grade)
                   VALUES ('{chatId}', '{grade}');";
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