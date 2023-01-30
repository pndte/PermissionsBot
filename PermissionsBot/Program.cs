using PermissionsBot;
using PermissionsBot.Bouncer;
using PermissionsBot.CommandHandler;
using PermissionsBot.DB;
using PermissionsBot.Logger;
using PermissionsBot.Sender;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static Logger _logger;
    private static Sender _sender;
    private static Bouncer _bouncer;
    private static CommandHandler _commandHandler;
    private static UserDatabase _userDatabase;
    private static ChatDatabase _chatDatabase;

    public static async Task Main(string[] args)
    {
        string? token;

        try
        {
            using (StreamReader sr = new StreamReader("token.txt"))
            {
                token = sr.ReadLine();
                if (token == null)
                {
                    return;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Cannot find the token file!");
            Console.WriteLine(e.Message);
            Console.ReadLine();
            return;
        }

        var botClient = new TelegramBotClient(token);
        List<BotCommand> commands = new List<BotCommand>()
        {
            new BotCommand() { Command = "/register", Description = "Зарегистрироваться, используя токен доступа." },
            new BotCommand()
                { Command = "/sendmessage", Description = "Разослать сообщение по ВСЕМ подписанным чатам." },
            new BotCommand()
                { Command = "/sendmessageto", Description = "Разослать сообщение по ВСЕМ УКАЗАННЫМ чатам." },
            new BotCommand() { Command = "/makesilent", Description = "Отключить callback-сообщения бота от чата." },
            new BotCommand() { Command = "/subscribe", Description = "Подписать чат на рассылку сообщений." },
            new BotCommand() { Command = "/unsubscribe", Description = "Отписать чат от рассылки сообщений." },
            new BotCommand() { Command = "/addteachertoken", Description = "Сгенерировать токен доступа для учителя." },
            new BotCommand() { Command = "/removetoken", Description = "Удалить токен доступа." },
            new BotCommand() { Command = "/addadmintoken", Description = "Сгенерировать токен доступа для админа." },
            new BotCommand() { Command = "/showalltokens", Description = "Показать все токены доступа." },
        };
        await botClient.SetMyCommandsAsync(commands);
        botClient.StartReceiving(updateHandler: UpdateHandler, pollingErrorHandler: PollingErrorHandler);

        _logger = new Logger();
        _bouncer = new Bouncer(_logger);
        _userDatabase = new UserDatabase("userdata");
        _chatDatabase = new ChatDatabase("chatdata");
        _sender = new Sender(botClient, _chatDatabase);
        _commandHandler = new CommandHandler(_logger, _sender, _userDatabase, _chatDatabase);
        Console.ReadLine();
    }

    private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.CallbackQuery != null)
        {
            await HandleButtonClick(update.CallbackQuery);
            return;
        }

        Message? message = update.Message;
        if (!_bouncer.CheckIfMessageIsCorrect(message))
        {
            return;
        }

        Command permissions = _userDatabase.GetPermissions(message.From.Id);
        if (permissions == Permissions.ADMIN)
        {
            await bot.SendTextMessageAsync(message.Chat.Id, "Cum", replyMarkup: Buttons.SENDMESSAGETO_MENU);
        }
        else if (permissions == Permissions.TEACHER)
        {
            await bot.SendTextMessageAsync(message.Chat.Id, "Cum", replyMarkup: Buttons.TEACHER_MAIN_MENU);
        }

        string[] args = message.Text.Split(' ');
        if (!_bouncer.CheckIfCommandIsCorrect(args[0]))
        {
            return;
        }

        Command command = _bouncer.GetCommandFromString(args[0]).Value;

        if (!_bouncer.CheckForPermission(message.Chat.Id, _userDatabase.GetPermissions(message.From.Id), command))
        {
            _sender.SendBack(message.Chat.Id,
                "Ошибка: отказано в доступе."); // TODO: придумать чё-то с ошибками (более нормированное).
            return;
        }

        _commandHandler.HandleCommand(command, message);
    }

    private static async Task HandleButtonClick(CallbackQuery callbackQuery)
    {
        string[] dataString = callbackQuery.Data.Split('_');
        Command firstData = (Command)Int32.Parse(dataString[0]);
        int secondData = 0;
        if (dataString.Length > 1) secondData = Int32.Parse(dataString[1]);
        switch (dataString.Length)
        {
            case 1:
                switch (firstData)
                {
                    case Command.SendMessage:
                        break;
                    case Command.SendMessageTo:
                        break;
                    case Command.CreateTeacherToken:
                        break;
                    case Command.RemoveToken:
                        break;
                    case Command.ShowAllTokens:
                        break;
                }
                break;
            case 2:
                switch (firstData)
                {
                    case Command.SendMessageTo:
                        switch (secondData)
                        {
                            case 12:
                                // TODO: здесь кнопка назад.
                                break;
                            default:
                                // TODO: здесь с первого по одиннадцатый.
                            break;
                        }
                        break;
                }
                break;
        }
    }

    private static Task PollingErrorHandler(ITelegramBotClient bot, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.ToString());
        return Task.CompletedTask;
    }
}