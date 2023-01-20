using PermissionsBot;
using PermissionsBot.Bouncer;
using PermissionsBot.CommandHandler;
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
        string token;

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

        Console.ReadLine();
    }

    private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        Message message = update.Message;
        if (message == null)
            return;

        if (!_bouncer.CheckForPermission(message.Chat.Id, Command.All, Command.SendMessage))
        {
            _sender.SendBack(message.Chat.Id, "Ошибка: отказано в доступе."); // TODO: придумать чё-то с ошибками (более нормированное).
            return;
        }
        
        if (message.From == null)
            return;

        if (message.Text == null)
        {
            await bot.SendTextMessageAsync(message.Chat.Id, "Invalid command.");
            return;
        }

        long userId = message.From.Id;
        Command command = Command.None;
        if (_permissionsMap.ContainsKey(userId))
        {
            command = _permissionsMap[userId];
        }
        else
        {
            _permissionsMap.Add(userId, command);
        }

        if (!message.Text.StartsWith('/'))
        {
            return;
        }

        /*string text = HandleCommandSignature(message.Text);

        if (_chatCommands.Contains(text))
        {
            HandleChatCommand(message, bot);
            return;
        }

        if (_userSettingsCommands.Contains(text))
        {
            HandleUserSettingsCommand(message, bot);
            return;
        }*/

        await bot.SendTextMessageAsync(message.Chat.Id, "Invalid command");
    }

    /*private static void HandleUserSettingsCommand(Message message, ITelegramBotClient bot)
    {
        SettingsCommand settingsCommand = Enum.Parse<SettingsCommand>(HandleCommandSignature(message.Text));
        Permissions permissions;
        switch (settingsCommand)
        {
            case SettingsCommand.make_default:
                MakeDefault(out permissions);
                _permissionsMap[message.From.Id] = permissions;
                bot.SendTextMessageAsync(message.Chat.Id,
                    $"You're now a default user! Your permissions: {permissions}");
                break;
            case SettingsCommand.make_moderator:
                MakeModerator(out permissions);
                _permissionsMap[message.From.Id] = permissions;
                bot.SendTextMessageAsync(message.Chat.Id, $"You're now a moderator! Your permissions: {permissions}");
                break;
            case SettingsCommand.make_admin:
                MakeAdmin(out permissions);
                _permissionsMap[message.From.Id] = permissions;
                bot.SendTextMessageAsync(message.Chat.Id, $"You're now an admin! Your permissions: {permissions}");
                break;
        }
    }

    private static string HandleCommandSignature(string command)
    {
        return command.Substring(1);
    }

    private static void MakeDefault(out Permissions permissions)
    {
        permissions = Permissions.read_message;
        permissions |= Permissions.write_message;
        permissions |= Permissions.record_voice_message;
    }

    private static void MakeModerator(out Permissions permissions)
    {
        MakeDefault(out permissions);
        permissions |= Permissions.delete_message;
        permissions |= Permissions.pin_message;
        permissions |= Permissions.mute_user;
    }

    private static void MakeAdmin(out Permissions permissions)
    {
        permissions = Permissions.all;
    }


    private static void HandleChatCommand(Message message, ITelegramBotClient bot)
    {
        Permissions permissions = Enum.Parse<Permissions>(HandleCommandSignature(message.Text));
        Permissions userPermissions = _permissionsMap[message.From.Id];
        if (_permissionsMap[message.From.Id].HasFlag(permissions))
        {
            bot.SendTextMessageAsync(message.Chat.Id, "Success!");
            return;
        }

        bot.SendTextMessageAsync(message.Chat.Id, $"Access is denied! Permissions you have: {userPermissions}");
    }*/


    private static Task PollingErrorHandler(ITelegramBotClient bot, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }
}