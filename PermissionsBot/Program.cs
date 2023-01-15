using PermissionsBot;
using Telegram.Bot;
using Telegram.Bot.Types;

class Program
{
    private static string[] _chatCommands = new[]
    {
        Permissions.ReadMessage.ToString(),
        Permissions.WriteMessage.ToString(),
        Permissions.RecordVoiceMessage.ToString(),
        Permissions.DeleteMessage.ToString(),
        Permissions.PinMassage.ToString(),
        Permissions.AddUser.ToString(),
        Permissions.KickUser.ToString(),
        Permissions.MuteUser.ToString(),
    };

    public static void Main(string[] args)
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
        Console.ReadLine();
    }

    private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        Message message = update.Message;
        if (message == null)
        {
            return;
        }

        if (message.Text == null)
        {
            await bot.SendTextMessageAsync(message.Chat.Id, "Invalid command.");
            return;
        }

        foreach (var command in _chatCommands)
        {
            if (message.Text == command)
            {
                HandleCommand(message, bot);
                return;
            }
        }

        await bot.SendTextMessageAsync(message.Chat.Id, "Invalid command");
    }

    private static void HandleCommand(Message message, ITelegramBotClient bot)
    {
        Permissions permissions = Enum.Parse<Permissions>(message.Text);
        bot.SendTextMessageAsync(message.Chat.Id, permissions.ToString());
    }


    private static Task PollingErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
    {
        throw new NotImplementedException();
    }
}