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
                    { CallbackData = Command.SendMessageTo.GetHashCode().ToString() },
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
                { CallbackData = Command.CreateTeacherToken.GetHashCode().ToString() },
        },
        new InlineKeyboardButton[]
        {
            new InlineKeyboardButton("Удалить токен доступа")
                { CallbackData = Command.RemoveToken.GetHashCode().ToString() },
            new InlineKeyboardButton("Показать все токены доступа")
                { CallbackData = Command.ShowAllTokens.GetHashCode().ToString() },
        }
    });

    public static readonly InlineKeyboardMarkup SENDMESSAGETO_MENU = new InlineKeyboardMarkup(
        new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("1 класс") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_1" },
                new InlineKeyboardButton("2 класс") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_2" },
                new InlineKeyboardButton("3 класс") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_3" },
                new InlineKeyboardButton("4 класс") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_4" },
            },
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("5 класс") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_5" },
                new InlineKeyboardButton("6 класс") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_6" },
                new InlineKeyboardButton("7 класс") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_7" },
                new InlineKeyboardButton("8 класс") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_8" },
            },
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("9 класс") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_9" },
                new InlineKeyboardButton("10 класс") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_10" },
                new InlineKeyboardButton("11 класс") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_11" },
            },
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton("Назад") { CallbackData = $"{Command.SendMessageTo.GetHashCode()}_12" }
            }
        });
}