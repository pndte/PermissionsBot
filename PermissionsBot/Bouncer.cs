namespace PermissionsBot.Bouncer;

using Logger;
using Sender;

/// <summary>
/// Проверяет, имеет ли пользователь права на использование той или иной команды.
/// </summary>
public class Bouncer
{
    public Bouncer(Logger logger)
    {
        _logger = logger;
    }

    private Logger _logger;

    private Dictionary<string, Command> _commandsMap = new Dictionary<string, Command>() // TODO: убрать в другой класс.
    {
        { "/sendmessage", Command.SendMessage },
        { "/addteachertoken", Command.AddTeacherToken },
        { "/removeteachertoken", Command.RemoveTeacherToken },
        { "/addadmintoken", Command.AddAdminToken },
        { "/removeadmintoken", Command.RemoveAdminToken }
    };

    public Command? GetCommandFromString(string text)
    {
        if (_commandsMap.ContainsKey(text))
        {
            return _commandsMap[text];
        }

        return null;
    }

    public bool CheckForPermission(long chatId, Command userPerms, Command command)
    {
        if (userPerms.HasFlag(command)) // TODO: доделать.
        {
            _logger.Log($"User has {command.ToString()} flag!");
            return true;
        }

        _logger.LogWarning($"User has not {command.ToString()} flag!");
        return false;
    }
}