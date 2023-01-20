using Telegram.Bot.Types.Enums;

namespace PermissionsBot.CommandHandler;

using Telegram.Bot.Types;
using Sender;
using Logger;

/// <summary>
/// Обрабатывает команды. (!!!Должен использоваться только после проверки Bouncer-ом!!!).
/// </summary>
public class CommandHandler
{
    public CommandHandler(Logger logger, Sender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    private Logger _logger;
    private Sender _sender;

    public void HandleComand(Command command, Message message)
    {
        switch (command)
        {
            case Command.SendMessage:
                SendMessage(message);
                break;
            case Command.SilentChat:
                break;
            case Command.SubscribeChat:
                if (message.Chat.Type != ChatType.Group)
                {
                    _sender.SendBack(message.Chat.Id,
                        "Подписать чат можно только в том случае, если он является групповым.");
                    return;
                }
                break;
            case Command.AddTeacherToken:
                break;
            case Command.RemoveTeacherToken:
                break;
            case Command.AddAdminToken:
                break;
            case Command.RemoveAdminToken:
                break;
            case Command.ShowAllTokens:
                break;
        }
    }

    private void SendMessage(Message message)
    {
        Message repliedMessage = message.ReplyToMessage;
        if (repliedMessage == null)
        {
            _sender.SendBack(message.Chat.Id, "Пожалуйста, перешлите сообщение, которое будет отправлено.");
            return;
        }

        _sender.SendOutMessage(repliedMessage);
    }
}