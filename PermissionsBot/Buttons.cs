using Telegram.Bot.Types.ReplyMarkups;

namespace PermissionsBot;

public static class Buttons
{
    public static readonly InlineKeyboardMarkup TEACHER_MAIN_MENU = new InlineKeyboardMarkup(
        new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("Сообщение всем")
                    { CallbackData = Command.SendMessage.GetHashCode().ToString() },
                new InlineKeyboardButton("Сообщение классу...")
                    { CallbackData = Actions.SEND_MESSAGE_TO },
            }
        });

    public static readonly InlineKeyboardMarkup ADMIN_MAIN_MENU = new InlineKeyboardMarkup(new InlineKeyboardButton[][]
    {
        new InlineKeyboardButton[]
        {
            new InlineKeyboardButton("Сообщение всем") { CallbackData = Command.SendMessage.GetHashCode().ToString() },
            new InlineKeyboardButton("Сообщение классу...")
                { CallbackData = Command.SendMessageTo.GetHashCode().ToString() },
        },
        new InlineKeyboardButton[]
        {
            new InlineKeyboardButton("Создать токен доступа")
                { CallbackData = Actions.CREATE_TOKEN },
        },
        new InlineKeyboardButton[]
        {
            new InlineKeyboardButton("Удалить токен доступа")
                { CallbackData = Command.RemoveToken.GetHashCode().ToString() },
            new InlineKeyboardButton("Показать все токены доступа")
                { CallbackData = Command.ShowAllTokens.GetHashCode().ToString() },
        }
    });

    public static readonly InlineKeyboardMarkup TOKEN_MENU = new InlineKeyboardMarkup(
        new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("Создать токен учителя") { CallbackData = Actions.CREATE_TOKEN_TEACHER},
                new InlineKeyboardButton("Создать токен администратора") { CallbackData = Actions.CREATE_TOKEN_ADMIN },
            },
            
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("Назад") { CallbackData = $"{Actions.CREATE_TOKEN}_3" }, // TODO: фикс.
            },
        });

    public static readonly InlineKeyboardMarkup SENDMESSAGETO_MENU = new InlineKeyboardMarkup(
        new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("1 класс") { CallbackData = Actions.SEND_MESSAGE_TO_1,},
                new InlineKeyboardButton("2 класс") { CallbackData = Actions.SEND_MESSAGE_TO_2 },
                new InlineKeyboardButton("3 класс") { CallbackData = Actions.SEND_MESSAGE_TO_3 },
                new InlineKeyboardButton("4 класс") { CallbackData = Actions.SEND_MESSAGE_TO_4 },
            },
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("5 класс") { CallbackData = Actions.SEND_MESSAGE_TO_5 },
                new InlineKeyboardButton("6 класс") { CallbackData = Actions.SEND_MESSAGE_TO_6 },
                new InlineKeyboardButton("7 класс") { CallbackData = Actions.SEND_MESSAGE_TO_7 },
                new InlineKeyboardButton("8 класс") { CallbackData = Actions.SEND_MESSAGE_TO_8 },
            },
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("9 класс") { CallbackData = Actions.SEND_MESSAGE_TO_9 },
                new InlineKeyboardButton("10 класс") { CallbackData = Actions.SEND_MESSAGE_TO_10 },
                new InlineKeyboardButton("11 класс") { CallbackData = Actions.SEND_MESSAGE_TO_11 },
            },
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("Назад") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_12" }
            }
        });
}