using PermissionsBot.DB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PermissionsBot.Sender;

public class Sender
{
    public Sender(ITelegramBotClient botClient, ChatDatabase chatDatabase)
    {
        _botClient = botClient;
        _chatDatabase = chatDatabase;
    }

    private ITelegramBotClient _botClient;
    private ChatDatabase _chatDatabase;

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
        List<long> chatIds = _chatDatabase.GetAllChats();
        if(chatIds.Count == 0) return;
        foreach (var chatId in chatIds)
        {
            await _botClient.ForwardMessageAsync(chatId, message.Chat.Id, message.MessageId);
        }
    }

    public async void SendOutMessageTo(Message message, byte grade)
    {
        List<long> chatIds = _chatDatabase.GetAllChatsByGrade(grade);
        if(chatIds.Count == 0) return;
        foreach (var chatId in chatIds)
        {
            await _botClient.ForwardMessageAsync(chatId, message.Chat.Id, message.MessageId);
        }
    }
    
    public async void SendOutMessageTo(Message message, byte[] grades)
    {
        List<long> chatIds = _chatDatabase.GetAllChatsByGrades(grades);
        if(chatIds.Count == 0) return;
        foreach (var chatId in chatIds)
        {
            await _botClient.ForwardMessageAsync(chatId, message.Chat.Id, message.MessageId);
        }
    }

    public async void SendOutMessage(string text)
    {
    }
}