using Microsoft.Data.Sqlite;

namespace PermissionsBot.DB;

public class UserDatabase: AbstractDatabase
{
    public UserDatabase(string tableName)
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

    public Command GetPermissions(long userId)
    {
        Command? permissions = null;
        ConnectAndRun((connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT permissions 
                    FROM {_tableName} 
                    WHERE userid = '{userId}'";
            permissions = (Command)Convert.ToInt32(command.ExecuteScalar());
        }));
        if (permissions == Command.None)
        {
            return Command.Register;
        }

        return permissions.Value;
    }

    public void AddToken(string token)
    {
        string[] args = token.Split('_');
        if (args.Length != 2)
        {
            return;
        }

        Command permissions = Command.None;
        switch (args[1])
        {
            case "TEACHER":
                permissions = Permissions.TEACHER;
                break;
            case "ADMIN":
                permissions = Permissions.ADMIN;
                break;
            default:
                return;
        }

        ConnectAndRun((SqliteConnection connection) =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"INSERT INTO {_tableName} (token, permissions)
                   VALUES ('{token}', '{permissions.GetHashCode()}');";
            command.ExecuteNonQueryAsync();
        });
    }

    public bool ContainFreeToken(string token)
    {
        bool result = false;
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT token FROM {_tableName} WHERE token = '{token}' AND userid IS NULL";
            if (command.ExecuteScalar() != null)
            {
                result = true;
            }
        });
        return result;
    }

    public bool ContainToken(string token)
    {
        bool result = false;
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT token FROM {_tableName} WHERE token = '{token}'";
            if (command.ExecuteScalar() != null)
            {
                result = true;
            }
        });
        return result;
    }

    public bool ContainUser(long userId)
    {
        bool result = false;
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT userid FROM {_tableName} WHERE userid = '{userId}'";
            if (command.ExecuteScalar() != null)
            {
                result = true;
            }
        });
        return result;
    }

    public void AddUserToToken(string token, long userId)
    {
        if (ContainUser(userId))
        {
            RemoveData(userId);
        }

        ConnectAndRun((SqliteConnection connection) =>
        {
            var replaceCommand = connection.CreateCommand();
            replaceCommand.CommandText =
                $@" UPDATE {_tableName}
                    SET userid = '{userId}'
                    WHERE token = '{token}' AND userid IS NULL;
                    ;";
            replaceCommand.ExecuteNonQuery();
        });
    }

    public void RemoveData(string token)
    {
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"DELETE FROM {_tableName} WHERE token = '{token}'";
            command.ExecuteNonQuery();
        });
    }

    public void RemoveData(long userId)
    {
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"DELETE FROM {_tableName} WHERE userid = '{userId}'";
            command.ExecuteNonQuery();
        });
    }

    public string[] ShowTable()
    {
        List<string> text = new List<string>();
        ConnectAndRun(connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT * FROM {_tableName} ORDER BY userid DESC, permissions ASC";
            using (var reader = command.ExecuteReader())
            {
                int index = 1;
                while (reader.Read())
                {
                    string token = reader.GetString(0);
                    long permissionsLong = long.Parse(reader.GetString(2));
                    string permissionsString;
                    string? userId = null;
                    if (!reader.IsDBNull(1))
                    {
                        userId = reader.GetString(1);
                    }

                    if (permissionsLong == -1)
                    {
                        permissionsString = "администратор";
                    }
                    else
                    {
                        permissionsString = "учитель";
                    }

                    if (userId == null)
                    {
                        text.Add(
                            $"{index++}\\) Токен: `{token}`\nID пользователя: НЕ ЗАРЕГИСТРИРОВАН\nУровень прав: *{permissionsString}*\n\n");
                    }
                    else
                    {
                        text.Add(
                            $"{index++}\\) Токен: `{token}`\nID пользователя: `{userId}`\nУровень прав: *{permissionsString}*\n\n");
                    }
                }
            }
        });
        return text.ToArray();
    }
}