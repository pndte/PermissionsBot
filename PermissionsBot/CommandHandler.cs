namespace PermissionsBot.CommandHandler;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Sender;
using Logger;
using Tokens;

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

    private readonly Logger _logger;
    private readonly Sender _sender;

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
                SubscribeChat(message);
                break;
            case Command.CreateTeacherToken:
                CreateTeacherToken(message);
                break;
            case Command.RemoveTeacherToken:
                // TODO: здесь удаляться из бд.
                break;
            case Command.CreateAdminToken:
                CreateAdminToken(message);
                break;
            case Command.RemoveAdminToken:
                break;
            case Command.ShowAllTokens:
                break;
        }
    }

    private void CreateAdminToken(Message message)
    {
        string adminAccessToken = TokenManager.CreateAdminAccessToken();
        _logger.Log($"Новый токен был создан!\n{adminAccessToken}");
        _sender.SendBack(message.Chat.Id, adminAccessToken);
    }

    private void CreateTeacherToken(Message message)
    {
        string teacherAccessToken = TokenManager.CreateTeacherAccessToken();
        _logger.Log($"Новый токен был создан!\n{teacherAccessToken}");
        // TODO: здесь объект должен заноситься в бд.
        _sender.SendBack(message.Chat.Id, teacherAccessToken);
    }

    private void SubscribeChat(Message message)
    {
        if (message.Chat.Type != ChatType.Group)
        {
            _sender.SendBack(message.Chat.Id,
                "Подписать чат можно только в том случае, если он является групповым.");
            return;
        }
        _sender.SendBack(message.Chat.Id, "Чат успешно подписан!");
        // TODO: чат добавляется в бд.
    }

    private void SendMessage(Message message)
    {
        Message? repliedMessage = message.ReplyToMessage;
        if (repliedMessage == null)
        {
            _sender.SendBack(message.Chat.Id, "Пожалуйста, перешлите сообщение, которое будет отправлено.");
            return;
        }
        _sender.SendBack(message.Chat.Id, repliedMessage.Text);
        _sender.SendOutMessage(repliedMessage);
    }
}