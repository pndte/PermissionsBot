using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PermissionsBot.Sender;

public class Sender
{
    public Sender(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    private ITelegramBotClient _botClient;

    /// <summary>
    /// Метод для ответа взаимодействующему с ботом пользователю.
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="message"></param>
    public async void SendBack(long chatId, Message message)
    {
    }

    public async void SendBack(long chatId, string text)
    {
        await _botClient.SendTextMessageAsync(chatId, text);
    }

    public async void SendBack(long chatId, string text, ParseMode parseMode)
    {
        await _botClient.SendTextMessageAsync(chatId, text, parseMode);
    }

    /// <summary>
    /// Метод для рассылки сообщений по всем подписанным чатам.
    /// </summary>
    /// <param name="message"></param>
    public async void SendOutMessage(Message message)
    {
    }

    public async void SendOutMessage(string text)
    {
    }
}