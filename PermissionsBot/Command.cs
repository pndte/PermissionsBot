namespace PermissionsBot;

/// <summary>
/// Используется как перечислитель для команд и как контейнер с правами пользователя.
/// </summary>
[Flags]
public enum Command : uint
{
    None = 0,
    Register = 1,
    SendMessage = 2,
    SendMessageTo = 4,
    SilentChat = 8,
    SubscribeChat = 16,
    CreateTeacherToken = 32,
    CreateAdminToken = 64,
    RemoveToken = 128,
    ShowAllTokens = 256,
    All = ~None,
}