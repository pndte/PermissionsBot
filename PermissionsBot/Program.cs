using PermissionsBot;
using PermissionsBot.Bouncer;
using PermissionsBot.CommandHandler;
using PermissionsBot.DB;
using PermissionsBot.Logger;
using PermissionsBot.Sender;
using Telegram.Bot;
using Telegram.Bot.Types;

class Program
{
    private static Logger _logger;
    private static Sender _sender;
    private static Bouncer _bouncer;
    private static CommandHandler _commandHandler;
    private static UserDatabase _userDatabase;

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
            new BotCommand(){Command = "/register", Description = "1"},
            new BotCommand(){Command = "/sendmessage", Description = "2"},
            new BotCommand(){Command = "/sendmessageto", Description = "3"},
            new BotCommand(){Command = "/silentchat", Description = "4"},
            new BotCommand(){Command = "/subscribechat", Description = "5"},
            new BotCommand(){Command = "/addteachertoken", Description = "6"},
            new BotCommand(){Command = "/removetoken", Description = "7"},
            new BotCommand(){Command = "/addadmintoken", Description = "8"},
            new BotCommand(){Command = "/showalltokens", Description = "10"},
            
        };
        await botClient.SetMyCommandsAsync(commands);
        botClient.StartReceiving(updateHandler: UpdateHandler, pollingErrorHandler: PollingErrorHandler);

        _logger = new Logger();
        _sender = new Sender(botClient);
        _bouncer = new Bouncer(_logger);
        _userDatabase = new UserDatabase("userdata");
        _commandHandler = new CommandHandler(_logger, _sender, _userDatabase);
        Console.ReadLine();
    }

    private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        Message? message = update.Message;
        if (!_bouncer.CheckIfTheMessageIsCorrect(message))
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: неверно введена команда."); // TODO: перенести внутрь вышибалы.
            return;
        }

        string[] args = message.Text.Split(' ');
        Command command = _bouncer.GetCommandFromString(args[0]).Value;

        if (!_bouncer.CheckForPermission(message.Chat.Id, _userDatabase.GetPermissions(message.From.Id), command))
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: отказано в доступе."); // TODO: придумать чё-то с ошибками (более нормированное).
            return;
        }
        
        _commandHandler.HandleCommand(command, message);
    }

    private static Task PollingErrorHandler(ITelegramBotClient bot, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.ToString());
        return Task.CompletedTask;
    }
}