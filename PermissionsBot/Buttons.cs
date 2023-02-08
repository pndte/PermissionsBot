using Telegram.Bot.Types.ReplyMarkups;

namespace PermissionsBot;

public static class Buttons
{
    public static readonly InlineKeyboardMarkup REGISTER_MENU = new InlineKeyboardMarkup(
        new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton(Program.Texts.GetButtonText("register"))
                    { CallbackData = Actions.REGISTER },
            }
        });

    public static readonly InlineKeyboardMarkup TEACHER_MAIN_MENU = new InlineKeyboardMarkup(
        new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton(Program.Texts.GetButtonText("sendmessage"))
                    { CallbackData = Actions.SEND_MESSAGE },
                new InlineKeyboardButton(Program.Texts.GetButtonText("sendmessageto"))
                    { CallbackData = Actions.SEND_MESSAGE_TO },
            }
        });

    public static readonly InlineKeyboardMarkup ADMIN_MAIN_MENU = new InlineKeyboardMarkup(new InlineKeyboardButton[][]
    {
        new InlineKeyboardButton[]
        {
            new InlineKeyboardButton(Program.Texts.GetButtonText("sendmessage"))
                { CallbackData = Actions.SEND_MESSAGE },
            new InlineKeyboardButton(Program.Texts.GetButtonText("sendmessageto"))
                { CallbackData = Actions.SEND_MESSAGE_TO },
        },
        new InlineKeyboardButton[]
        {
            new InlineKeyboardButton(Program.Texts.GetButtonText("createtoken"))
                { CallbackData = Actions.CREATE_TOKEN },
        },
        new InlineKeyboardButton[]
        {
            new InlineKeyboardButton(Program.Texts.GetButtonText("removetoken"))
                { CallbackData = Command.RemoveToken.GetHashCode().ToString() },
            new InlineKeyboardButton(Program.Texts.GetButtonText("showalltokens"))
                { CallbackData = Command.ShowAllTokens.GetHashCode().ToString() },
        }
    });

    public static readonly InlineKeyboardMarkup TOKEN_MENU = new InlineKeyboardMarkup(
        new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton(Program.Texts.GetButtonText("createtokenteacher"))
                    { CallbackData = Actions.CREATE_TOKEN_TEACHER },
                new InlineKeyboardButton(Program.Texts.GetButtonText("createtokenadmin"))
                    { CallbackData = Actions.CREATE_TOKEN_ADMIN },
            },

            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton(Program.Texts.GetButtonText("back"))
                    { CallbackData = Actions.CREATE_TOKEN_BACK }, // TODO: фикс.
            },
        });

    public static readonly InlineKeyboardMarkup SENDMESSAGETO_MENU = new InlineKeyboardMarkup(
        new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton(Program.Texts.GetButtonText("firstgrade")) { CallbackData = Actions.SEND_MESSAGE_TO_1, },
                new InlineKeyboardButton(Program.Texts.GetButtonText("secondgrade")) { CallbackData = Actions.SEND_MESSAGE_TO_2 },
                new InlineKeyboardButton(Program.Texts.GetButtonText("thirdgrade")) { CallbackData = Actions.SEND_MESSAGE_TO_3 },
                new InlineKeyboardButton(Program.Texts.GetButtonText("forthgrade")) { CallbackData = Actions.SEND_MESSAGE_TO_4 },
            },
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton(Program.Texts.GetButtonText("fifthgrade")) { CallbackData = Actions.SEND_MESSAGE_TO_5 },
                new InlineKeyboardButton(Program.Texts.GetButtonText("sixthgrade")) { CallbackData = Actions.SEND_MESSAGE_TO_6 },
                new InlineKeyboardButton(Program.Texts.GetButtonText("seventhgrade")) { CallbackData = Actions.SEND_MESSAGE_TO_7 },
                new InlineKeyboardButton(Program.Texts.GetButtonText("eighthgrade")) { CallbackData = Actions.SEND_MESSAGE_TO_8 },
            },
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton(Program.Texts.GetButtonText("ninthgrade")) { CallbackData = Actions.SEND_MESSAGE_TO_9 },
                new InlineKeyboardButton(Program.Texts.GetButtonText("tenthgrade")) { CallbackData = Actions.SEND_MESSAGE_TO_10 },
                new InlineKeyboardButton(Program.Texts.GetButtonText("eleventhgrade")) { CallbackData = Actions.SEND_MESSAGE_TO_11 },
            },
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton(Program.Texts.GetButtonText("back")) { CallbackData = Actions.SEND_MESSAGE_TO_BACK }
            }
        });
}