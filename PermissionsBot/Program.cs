using PermissionsBot;
using PermissionsBot.Tokens;
using PermissionsBot.Bouncer;
using PermissionsBot.CommandHandler;
using PermissionsBot.DB;
using PermissionsBot.Logger;
using PermissionsBot.Sender;
using Telegram.Bot;
using Telegram.Bot.Types;

class Program
{
    private static string[] _userSettingsCommands = new string[]
    {
        SettingsCommand.make_default.ToString(),
        SettingsCommand.make_moderator.ToString(),
        SettingsCommand.make_admin.ToString(),
    };

    private static Dictionary<long, Command> _permissionsMap = new Dictionary<long, Command>();

    private static Logger _logger;
    private static Sender _sender;
    private static Bouncer _bouncer;
    private static CommandHandler _commandHandler;

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
        botClient.StartReceiving(updateHandler: UpdateHandler, pollingErrorHandler: PollingErrorHandler);

        _logger = new Logger();
        _sender = new Sender(botClient);
        _bouncer = new Bouncer(_logger);
        _commandHandler = new CommandHandler(_logger, _sender);
        Console.WriteLine(TokenManager.CreateTeacherAccessToken());
        Console.WriteLine(TokenManager.CreateTeacherAccessToken());
        Console.WriteLine(TokenManager.CreateAdminAccessToken());
        Console.WriteLine(TokenManager.CreateAdminAccessToken());
        UserDatabase db = new UserDatabase("userdata");
        db.AddToken(TokenManager.CreateTeacherAccessToken());

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

        Command command = _bouncer.GetCommandFromString(message.Text).Value;

        if (!_bouncer.CheckForPermission(message.Chat.Id, Command.All, command))
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: отказано в доступе."); // TODO: придумать чё-то с ошибками (более нормированное).
            return;
        }
        
        _sender.SendBack(message.Chat.Id, "Доступ разрешён."); 
        _commandHandler.HandleComand(command, message);
    }

    private static Task PollingErrorHandler(ITelegramBotClient bot, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }
}