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
    public CommandHandler(Logger logger, Sender sender, UserDatabase userDatabase, ChatDatabase chatDatabase)
    {
        _logger = logger;
        _sender = sender;
        _userDatabase = userDatabase;
        _chatDatabase = chatDatabase;
    }

    private readonly Logger _logger;
    private readonly Sender _sender;
    private readonly UserDatabase _userDatabase;
    private readonly ChatDatabase _chatDatabase;

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
                SendMessageTo(message);
                break;
            case Command.MakeSilent:
                break;
            case Command.Subscribe:
                SubscribeChat(message); // TODO: дб.
                break;
            case Command.Unsubscribe:
                UnsubscribeChat(message);
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
                ShowAllTokens(message);
                break;
            default:
                _sender.SendBack(message.Chat.Id, "Ошибка: неопознанная команда.");
                return;
        }
    }

    private void UnsubscribeChat(Message message)
    {
        if (message.Chat.Type != ChatType.Group)
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: отписать от сообщений можно только групповой чат.");
            return;
        }

        if (!_chatDatabase.ContainChat(message.Chat.Id))
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: чат не подписан.");
            return;
        }

        _chatDatabase.RemoveData(message.Chat.Id);
        _sender.SendBack(message.Chat.Id, "Чат успешно отписан.");
    }

    private void SendMessageTo(Message message)
    {
        Message? repliedMessage = message.ReplyToMessage;
        if (repliedMessage == null)
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: перешлите сообщение, которое будет отправлено.");
            return;
        }

        string[] args = message.Text.Split(' ');
        if (args.Length != 2)
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: укажите номера получающих сообщения классов.");
            return;
        }

        string[] gradesStrings = args[1].Split(';');
        if (gradesStrings.Length > 1)
        {
            Byte[] grades = new byte[gradesStrings.Length];
            for (int i = 0; i < gradesStrings.Length; i++)
            {
                byte grade;
                if (!byte.TryParse(gradesStrings[i], out grade))
                {
                    _sender.SendBack(message.Chat.Id, "Ошибка: неверно введён номер класса.");
                    return;
                }

                if (grade > 11 || grade < 1)
                {
                    _sender.SendBack(message.Chat.Id, "Ошибка: неверно введён номер класса.");
                    return;
                }

                grades[i] = grade;
            }

            _sender.SendOutMessageTo(repliedMessage, grades);
            _sender.SendBack(message.Chat.Id, "Сообщение успешно разослано по указанным чатам.");
            return;
        }

        byte singleGrade; // TODO: сделать максимум в 11 классов.
        if (!byte.TryParse(args[1], out singleGrade))
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: неверно введён номер класса.");
            return;
        }

        if (singleGrade > 11 || singleGrade < 1)
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: неверно введён номер класса.");
            return;
        }

        _sender.SendOutMessageTo(repliedMessage, singleGrade);
        _sender.SendBack(message.Chat.Id, "Сообщение успешно разослано по указанным чатам.");
    }

    private void ShowAllTokens(Message message)
    {
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
                "Ошибка: подписать чат можно только в том случае, если он является групповым.");
            return;
        }

        string[] args = message.Text.Split(' ');
        if (args.Length != 2)
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: укажите номер класса.");
            return;
        }

        byte grade;
        if (!byte.TryParse(args[1], out grade))
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: неправильно указан класс.");
            return;
        }

        if (grade < 1 || grade > 11)
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: неправильно указан класс.");
            return;
        }

        _chatDatabase.AddChat(message.Chat.Id, grade);
        _sender.SendBack(message.Chat.Id, "Чат успешно подписан!");
        // TODO: сделать проверку на наличие чата.
    }

    private void SendMessage(Message message)
    {
        Message? repliedMessage = message.ReplyToMessage;
        if (repliedMessage == null)
        {
            _sender.SendBack(message.Chat.Id, "Пожалуйста, перешлите сообщение, которое будет отправлено.");
            return;
        }

        _sender.SendOutMessage(repliedMessage);
    }
}