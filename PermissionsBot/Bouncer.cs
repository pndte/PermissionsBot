using Telegram.Bot.Types;

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

    private Dictionary<string, Command> _commandsMap = new Dictionary<string, Command>()
    {
        {"/register", Command.Register},
        {"/sendmessage", Command.SendMessage},
        {"/sendmessageto", Command.SendMessageTo},
        {"/silentchat", Command.SilentChat},
        {"/subscribechat", Command.SubscribeChat},
        {"/addteachertoken", Command.CreateTeacherToken },
        {"/addadmintoken", Command.CreateAdminToken },
        {"/removetoken", Command.RemoveToken },
        {"/showalltokens", Command.ShowAllTokens},
    };

    public Command? GetCommandFromString(string text)
    {
        if (_commandsMap.ContainsKey(text))
        {
            return _commandsMap[text];
        }

        return null;
    }

    public bool CheckIfTheMessageIsCorrect(Message? message)
    {
        if (message == null)
        {
            return false;
        }

        if (message.From == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(message.Text))
        {
            return false;
        }

        string[] text = message.Text.Split(' ');

        if (!_commandsMap.ContainsKey(text[0]))
        {
            return false;
        }
        
        return true;
    }

    public bool CheckForPermission(long chatId, Command userPerms, Command command)
    {
        if (!userPerms.HasFlag(command))
        {
            _logger.LogWarning($"User has not {command.ToString()} flag!");
            return false;
        }

        _logger.Log($"User has {command.ToString()} flag!");
        return true;
    }
}