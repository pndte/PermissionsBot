using System.Data;
using Microsoft.Data.Sqlite;

namespace PermissionsBot.DB;

public class UserDatabase
{
    public UserDatabase(string databaseName)
    {
        _databaseName = databaseName;
        ConnectAndRun((connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                @$"CREATE TABLE IF NOT EXISTS {databaseName}                 
                   (token TEXT NOT NULL UNIQUE, userid INTEGER, permissions INTEGER);";
            command.ExecuteNonQuery();
        }));
    }

    private string _databaseName;

    public Command GetPermissions(long userId)
    {
        Command? permissions = null;
        ConnectAndRun((connection =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"SELECT permissions 
                    FROM {_databaseName} 
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
        ConnectAndRun((SqliteConnection connection) =>
        {
            var command = connection.CreateCommand();
            command.CommandText =
                $@"INSERT INTO {_databaseName} (token)
                   VALUES ('{token}');";
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
                $@"SELECT token FROM {_databaseName} WHERE token = '{token}' AND userid IS NULL";
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
                $@"SELECT userid FROM {_databaseName} WHERE userid = '{userId}'";
            if (command.ExecuteScalar() != null)
            {
                result = true;
            }
        });
        return result;
    }

    public void AddUserToToken(string token, long userId)
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

        if (ContainUser(userId))
        {
            ConnectAndRun(connection =>
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    $@"DELETE FROM {_databaseName} WHERE userid = '{userId}'";
                command.ExecuteNonQuery();
            }); // TODO: вынести в отдельный метод удаления.
        }

        ConnectAndRun((SqliteConnection connection) =>
        {
            var replaceCommand = connection.CreateCommand();
            replaceCommand.CommandText =
                $@" UPDATE {_databaseName}
                    SET userid = '{userId}', permissions = '{permissions.GetHashCode()}'
                    WHERE token = '{token}' AND userid IS NULL;
                    ;";
            replaceCommand.ExecuteNonQuery();
        });
    }

    private delegate void DatabaseProcedure(SqliteConnection connection);

    private void ConnectAndRun(DatabaseProcedure databaseProcedure)
    {
        using (var connection = new SqliteConnection($"Data Source=data.db; Mode=ReadWriteCreate;"))
        {
            connection.Open();
            databaseProcedure.Invoke(connection);
        }
    }
}