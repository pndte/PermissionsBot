using System.Text;

namespace PermissionsBot.Handlers.Commands;

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
                SubscribeChat(message);
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
                _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("unidentifiedcommand"));
                return;
        }
    }

    private void UnsubscribeChat(Message message)
    {
        if (message.Chat.Type != ChatType.Group)
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("subscribeprivatechat"));
            return;
        }

        if (!_chatDatabase.ContainChat(message.Chat.Id))
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("chatnotsubscribed"));
            return;
        }

        _chatDatabase.RemoveData(message.Chat.Id);
        _sender.SendBack(message.Chat.Id, Program.Texts.GetMessageText("unsubscribesuccess"));
    }

    private void SendMessageTo(Message message) 
    {
        Message? repliedMessage = message.ReplyToMessage;
        if (repliedMessage == null)
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("messagenotreplied"));
            return;
        }

        string[] args = message.Text.Split(' '); 
        if (args.Length != 2)
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("gradesnotspecified"));
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
                    _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("gradenumbernotcorrect"));
                    return;
                }

                if (grade > 11 || grade < 1)
                {
                    _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("gradenumbernotcorrect"));
                    return;
                }

                grades[i] = grade;
            }

            _sender.SendOutMessageTo(repliedMessage, grades); 
            _sender.SendBack(message.Chat.Id, Program.Texts.GetMessageText("sendmessagetosuccess"));
            return;
        }

        byte singleGrade; // TODO: сделать максимум в 11 классов.
        if (!byte.TryParse(args[1], out singleGrade))
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("gradenumbernotcorrect"));
            return;
        }

        if (singleGrade > 11 || singleGrade < 1)
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("gradenumbernotcorrect"));
            return;
        }

        _sender.SendOutMessageTo(repliedMessage, singleGrade);
        _sender.SendBack(message.Chat.Id, Program.Texts.GetMessageText("sendmessagetosuccess"));
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
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("tokennotentered"));
            return;
        }

        if (!_userDatabase.ContainToken(args[1]))
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("tokennotcorrect"));
            return;
        }

        _userDatabase.RemoveData(args[1]);
        _sender.SendBack(message.Chat.Id, Program.Texts.GetMessageText("tokenremove"));
    }

    private void Register(Message message)
    {
        string[] args = message.Text.Split(' ');
        if (args.Length != 2)
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("tokennotentered"));
            return;
        }

        if (!_userDatabase.ContainFreeToken(args[1]))
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("tokennotcorrect"));
            return;
        }

        long l = message.From.Id;
        _userDatabase.AddUserToToken(args[1], l);
        _sender.SendBack(message.Chat.Id, Program.Texts.GetMessageText("registersuccess"));
    }

    private void CreateAdminToken(Message message)
    {
        string adminAccessToken = TokenManager.CreateAdminAccessToken();
        _logger.Log($"Новый токен был создан!\n{adminAccessToken}");
        _userDatabase.AddToken(adminAccessToken);
        _sender.SendBack(message.Chat.Id,
            $"{Program.Texts.GetMessageText("admintokencreatedmarkdown")}`{adminAccessToken}`",
            ParseMode.MarkdownV2);
    }

    private void CreateTeacherToken(Message message)
    {
        string teacherAccessToken = TokenManager.CreateTeacherAccessToken();
        _logger.Log($"Новый токен был создан!\n{teacherAccessToken}");
        _userDatabase.AddToken(teacherAccessToken);
        _sender.SendBack(message.Chat.Id,
            $"{Program.Texts.GetMessageText("teachertokencreatedmarkdown")}`{teacherAccessToken}`",
            ParseMode.MarkdownV2);
    }

    private void SubscribeChat(Message message)
    {
        if (message.Chat.Type != ChatType.Group)
        {
            _sender.SendBack(message.Chat.Id,
                Program.Texts.GetErrorText("subscribeprivatechat"));
            return;
        }

        if (_chatDatabase.ContainChat(message.Chat.Id))
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("chatalreadysubscribed"));
            return;
        }

        string[] args = message.Text.Split(' ');
        if (args.Length != 2)
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("gradesnotspecified"));
            return;
        }

        byte grade;
        if (!byte.TryParse(args[1], out grade))
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("gradenumbernotcorrect"));
            return;
        }

        if (grade < 1 || grade > 11)
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("gradenumbernotcorrect"));
            return;
        }

        _chatDatabase.AddChat(message.Chat.Id, grade);
        _sender.SendBack(message.Chat.Id, Program.Texts.GetMessageText("subscribesuccess"));
        // TODO: сделать проверку на наличие чата.
    }

    private void SendMessage(Message message)
    {
        Message? repliedMessage = message.ReplyToMessage;
        if (repliedMessage == null)
        {
            _sender.SendBack(message.Chat.Id, Program.Texts.GetErrorText("messagenotreplied"));
            return;
        }

        _sender.SendOutMessage(repliedMessage);
    }
}