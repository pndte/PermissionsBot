using System.Text;

namespace PermissionsBot.CommandHandler;

using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Sender;
using Logger;
using Tokens;
using DB;

/// <summary>
/// Обрабатывает команды. (!!!Должен использоваться только после проверки Bouncer-ом!!!).
/// </summary>
public class CommandHandler
{
    public CommandHandler(Logger logger, Sender sender, UserDatabase userDatabase)
    {
        _logger = logger;
        _sender = sender;
        _userDatabase = userDatabase;
    }

    private readonly Logger _logger;
    private readonly Sender _sender;
    private readonly UserDatabase _userDatabase;

    public void HandleCommand(Command command, Message message)
    {
        switch (command)
        {
            case Command.Register:
                Register(message);
                break;
            case Command.SendMessage:
                SendMessage(message);
                break;
            case Command.SendMessageTo:
                break;
            case Command.SilentChat:
                break;
            case Command.SubscribeChat:
                SubscribeChat(message); // TODO: дб.
                break;
            case Command.CreateTeacherToken:
                CreateTeacherToken(message);
                break;
            case Command.CreateAdminToken:
                CreateAdminToken(message);
                break;
            case Command.RemoveToken:
                RemoveToken(message);
                break;
            case Command.ShowAllTokens:
                string[] text = _userDatabase.ShowTable();
                for (int i = 0; i < text.Length; i += 10)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int j = i; j < i + 10 && j < text.Length; j++)
                    {
                        stringBuilder.Append(text[j]);
                    }
                    _sender.SendBack(message.Chat.Id, stringBuilder.ToString(), ParseMode.MarkdownV2);
                }
                break;
            default:
                _sender.SendBack(message.Chat.Id, "Ошибка: неопознанная команда.");
                return;
        }
    }

    private void RemoveToken(Message message)
    {
        string[] args = message.Text.Split(' ');
        if (args.Length != 2)
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: укажите токен доступа.");
            return;
        }

        if (!_userDatabase.ContainToken(args[1]))
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: неверно введён токен доступа.");
            return;
        }

        _userDatabase.RemoveData(args[1]);
        _sender.SendBack(message.Chat.Id, "Токен доступа успешно удалён.");
    }

    private void Register(Message message)
    {
        string[] args = message.Text.Split(' ');
        if (args.Length != 2)
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: укажите токен доступа.");
            return;
        }

        if (!_userDatabase.ContainFreeToken(args[1]))
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: неверно введён токен доступа.");
            return;
        }

        long l = message.From.Id;
        _userDatabase.AddUserToToken(args[1], l);
        _sender.SendBack(message.Chat.Id, "Вы успешно зарегистрированы!");
    }

    private void CreateAdminToken(Message message)
    {
        string adminAccessToken = TokenManager.CreateAdminAccessToken();
        _logger.Log($"Новый токен был создан!\n{adminAccessToken}");
        _userDatabase.AddToken(adminAccessToken);
        _sender.SendBack(message.Chat.Id,
            $"Токен администратора создан\\. Используйте /register \\[ваш токен\\], чтобы зарегистрироваться\\.\n`{adminAccessToken}`",
            ParseMode.MarkdownV2);
    }

    private void CreateTeacherToken(Message message)
    {
        string teacherAccessToken = TokenManager.CreateTeacherAccessToken();
        _logger.Log($"Новый токен был создан!\n{teacherAccessToken}");
        _userDatabase.AddToken(teacherAccessToken);
        _sender.SendBack(message.Chat.Id,
            $"Токен учителя создан\\. Используйте /register \\[ваш токен\\], чтобы зарегистрироваться\\.\n`{teacherAccessToken}`",
            ParseMode.MarkdownV2);
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