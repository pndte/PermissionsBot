using System.Text;
using PermissionsBot;
using PermissionsBot.Bouncer;
using PermissionsBot.Handlers.Commands;
using PermissionsBot.DB;
using PermissionsBot.Logger;
using PermissionsBot.Sender;
using PermissionsBot.Tokens;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static TelegramBotClient _botClient;
    private static Logger _logger;
    private static Sender _sender;
    private static Bouncer _bouncer;
    private static CommandHandler _commandHandler;
    private static UserDatabase _userDatabase;
    private static ChatDatabase _chatDatabase;
    private static PlannedActionsDatabase _plannedActionsDatabase;
    
    public static Texts Texts = new Texts("texts.xml");

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

        _botClient = new TelegramBotClient(token);
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
        await _botClient.SetMyCommandsAsync(commands);
        _botClient.StartReceiving(updateHandler: UpdateHandler, pollingErrorHandler: PollingErrorHandler);

        _logger = new Logger();         
        _bouncer = new Bouncer(_logger);
        _userDatabase = new UserDatabase("userdata");
        _chatDatabase = new ChatDatabase("chatdata");
        _plannedActionsDatabase = new PlannedActionsDatabase("actionsdata");
        _sender = new Sender(_botClient, _chatDatabase);
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

        if (_plannedActionsDatabase.ContainUser(message.From.Id))
        {
            long userId = message.From.Id;
            HandlePlannedAction(message, _plannedActionsDatabase.GetBotMessageId(userId),
                _plannedActionsDatabase.GetAction(userId));
            return;
        }

        string[] args = message.Text.Split(' ');
        if (!_bouncer.CheckIfCommandIsCorrect(args[0]))
        {
            Command permissions = _userDatabase.GetPermissions(message.From.Id);
            if (permissions == Permissions.ADMIN)
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Панель управления.",
                    replyMarkup: Buttons.ADMIN_MAIN_MENU);
                return;
            }
            else if (permissions == Permissions.TEACHER)
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Панель управления.",
                    replyMarkup: Buttons.TEACHER_MAIN_MENU);
                return;
            }
            await bot.SendTextMessageAsync(message.Chat.Id, "Регистрация",
                replyMarkup: Buttons.REGISTER_MENU);
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
        byte secondData = 0;
        if (dataString.Length > 1) secondData = byte.Parse(dataString[1]);
        switch (dataString.Length)
        {
            case 1:
                switch (firstData)
                {
                    case Command.Register:
                        _sender.EditTextMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                            "Введите токен доступа.");
                        _plannedActionsDatabase.AddUser(callbackQuery.From.Id,
                            Actions.REGISTER, 0, callbackQuery.Message.MessageId, callbackQuery.Message.Chat.Id);
                        break;
                    case Command.SendMessage: // TODO: вынести в отдельный метод.
                        _sender.EditTextMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                            "Ответьте на сообщение, которое будет отправлено.");
                        _plannedActionsDatabase.AddUser(callbackQuery.From.Id,
                            Actions.SEND_MESSAGE, 0, callbackQuery.Message.MessageId, callbackQuery.Message.Chat.Id);
                        break;
                    case Command.SendMessageTo:
                        _sender.EditTextMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                            "Ответьте на сообщение, которое будет отправлено.");
                        _plannedActionsDatabase.AddUser(callbackQuery.From.Id,
                            Actions.SEND_MESSAGE_TO, 0, callbackQuery.Message.MessageId, callbackQuery.Message.Chat.Id);
                        break;
                    case Command.CreateTeacherToken:
                        _sender.EditTextMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                            "Выберите тип создаваемого токена.", markup: Buttons.TOKEN_MENU);
                        _plannedActionsDatabase.AddUser(callbackQuery.From.Id,
                            Actions.CREATE_TOKEN, 0, callbackQuery.Message.MessageId, callbackQuery.Message.Chat.Id);
                        break;
                    case Command.RemoveToken:
                        _sender.EditTextMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                            "Укажите токен, который будет удалён из базы данных.");
                        _plannedActionsDatabase.AddUser(callbackQuery.From.Id,
                            Actions.REMOVE_TOKEN, 0, callbackQuery.Message.MessageId, callbackQuery.Message.Chat.Id);
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

                            _sender.SendBack(callbackQuery.Message.Chat.Id, stringBuilder.ToString(),
                                ParseMode.MarkdownV2);
                        }

                        InlineKeyboardMarkup markup = Buttons.TEACHER_MAIN_MENU;
                        if (_userDatabase.GetPermissions(callbackQuery.From.Id) == Permissions.ADMIN)
                        {
                            markup = Buttons.ADMIN_MAIN_MENU;
                        }

                        await _sender.SendBack(callbackQuery.Message.Chat.Id,
                            "Панель управления.",
                            markup: markup);
                        await _botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId);
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
                                InlineKeyboardMarkup markup = Buttons.TEACHER_MAIN_MENU;
                                if (_userDatabase.GetPermissions(callbackQuery.From.Id) == Permissions.ADMIN)
                                {
                                    markup = Buttons.ADMIN_MAIN_MENU;
                                }

                                _sender.EditTextMessageAsync(callbackQuery.Message.Chat.Id,
                                    callbackQuery.Message.MessageId,
                                    "Панель управления.",
                                    markup: markup);
                                _plannedActionsDatabase.RemoveData(callbackQuery.From.Id);
                                break;
                            default:
                                _sender.SendOutMessageTo(callbackQuery.Message.Chat.Id,
                                    _plannedActionsDatabase.GetUserMessageId(callbackQuery.From.Id), secondData);
                                break;
                        }

                        break;
                    case Command.CreateTeacherToken:
                        string token;
                        switch (secondData)
                        {
                            case 1:
                                token = TokenManager.CreateTeacherAccessToken();
                                _userDatabase.AddToken(token); // TODO: перенести в нормальный класс и сделать красиво.
                                _sender.SendBack(callbackQuery.Message.Chat.Id,
                                    $"Токен учителя создан\\. Используйте /register \\[ваш токен\\], чтобы зарегистрироваться\\.\n`{token}`",
                                    ParseMode.MarkdownV2);
                                break;
                            case 2:
                                token = TokenManager.CreateAdminAccessToken();
                                _userDatabase.AddToken(token); // TODO: перенести в нормальный класс и сделать красиво.
                                _sender.SendBack(callbackQuery.Message.Chat.Id,
                                    $"Токен администратора создан\\. Используйте /register \\[ваш токен\\], чтобы зарегистрироваться\\.\n`{token}`",
                                    ParseMode.MarkdownV2);
                                break;
                            case 3: // Это назад.
                                InlineKeyboardMarkup markup = Buttons.TEACHER_MAIN_MENU;
                                if (_userDatabase.GetPermissions(callbackQuery.From.Id) == Permissions.ADMIN)
                                {
                                    markup = Buttons.ADMIN_MAIN_MENU;
                                }

                                await _sender.SendBack(callbackQuery.Message.Chat.Id,
                                    "Панель управления.",
                                    markup: markup);
                                await _botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id,
                                    callbackQuery.Message.MessageId);
                                _plannedActionsDatabase.RemoveData(callbackQuery.From.Id);
                                break;
                        }

                        break;
                }

                break;
        }
    }

    private static async Task HandlePlannedAction(Message userMessage, int botMessageId, string action)
    {
        string[] args = action.Split('_');
        Command firstArg = (Command)uint.Parse(args[0]);
        int secondArg = 0;
        switch (args.Length)
        {
            case 1:
                switch (firstArg)
                {
                    case Command.Register:
                        string messageTextRegister = userMessage.Text;

                        if (!_userDatabase.ContainFreeToken(messageTextRegister))
                        {
                            _sender.SendBack(userMessage.Chat.Id, "Ошибка: неверно введён токен доступа.");
                            return;
                        }

                        long userId = userMessage.From.Id;
                        _userDatabase.AddUserToToken(messageTextRegister, userId);
                        _sender.SendBack(userMessage.Chat.Id, "Вы успешно зарегистрированы!");
                        _plannedActionsDatabase.RemoveData(userMessage.From.Id);
                        Command permissionsRegister = _userDatabase.GetPermissions(userMessage.From.Id);
                        if (permissionsRegister == Permissions.ADMIN)
                        {
                            await _botClient.SendTextMessageAsync(userMessage.Chat.Id, "Панель управления.",
                                replyMarkup: Buttons.ADMIN_MAIN_MENU);
                        }
                        else if (permissionsRegister == Permissions.TEACHER)
                        {
                            await _botClient.SendTextMessageAsync(userMessage.Chat.Id, "Панель управления.",
                                replyMarkup: Buttons.TEACHER_MAIN_MENU);
                        }
                        break;
                    case Command.SendMessage:
                        if (userMessage.ReplyToMessage == null) // TODO: переделать.
                        {
                            return;
                        }

                        _sender.SendOutMessage(userMessage.ReplyToMessage);
                        InlineKeyboardMarkup markup = Buttons.TEACHER_MAIN_MENU;
                        if (_userDatabase.GetPermissions(userMessage.From.Id) == Permissions.ADMIN)
                        {
                            markup = Buttons.ADMIN_MAIN_MENU;
                        }

                        await _sender.EditTextMessageAsync(userMessage.Chat.Id,
                            botMessageId,
                            "Панель управления.",
                            markup: markup);
                        _plannedActionsDatabase.RemoveData(userMessage.From.Id);
                        await _botClient.DeleteMessageAsync(userMessage.Chat.Id,
                            userMessage.MessageId); // TODO: перенести в сендера
                        break;
                    case Command.SendMessageTo:
                        if (userMessage.ReplyToMessage == null)
                        {
                            return;
                        }

                        _plannedActionsDatabase.AddUser(userMessage.From.Id, Actions.SEND_MESSAGE_TO_GRADE_MENU,
                            userMessage.ReplyToMessage.MessageId, botMessageId, userMessage.Chat.Id);
                        await _sender.EditTextMessageAsync(userMessage.Chat.Id, botMessageId,
                            "Выберите чаты классов, в которые будут отправлены сообщения.",
                            markup: Buttons.SENDMESSAGETO_MENU);
                        await _botClient.DeleteMessageAsync(userMessage.Chat.Id, userMessage.MessageId);
                        break;
                    case Command.RemoveToken:
                        Command permissions = _userDatabase.GetPermissions(userMessage.From.Id);
                        InlineKeyboardMarkup markupToSend = Buttons.TEACHER_MAIN_MENU;
                        if (permissions == Permissions.ADMIN)
                        {
                            markupToSend = Buttons.ADMIN_MAIN_MENU;
                        }

                        string messageText = userMessage.Text; // TODO: дубляж кода из command handler, убрать.

                        if (!_userDatabase.ContainToken(messageText))
                        {
                            await _sender.SendBack(userMessage.Chat.Id, "Ошибка: неверно введён токен доступа.");
                            await _sender.SendBack(userMessage.Chat.Id, "Панель управления.",
                                markup: markupToSend);
                            _plannedActionsDatabase.RemoveData(userMessage.From.Id);
                            return;
                        }

                        _userDatabase.RemoveData(messageText);
                        await _sender.SendBack(userMessage.Chat.Id, "Токен доступа успешно удалён.");
                        _plannedActionsDatabase.RemoveData(userMessage.From.Id);
                        await _sender.SendBack(userMessage.Chat.Id, "Панель управления.",
                            markup: markupToSend);
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