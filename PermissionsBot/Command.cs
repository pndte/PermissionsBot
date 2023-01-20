namespace PermissionsBot;

/// <summary>
/// Используется как перечислитель для команд и как контейнер с правами пользователя.
/// </summary>
[Flags]
public enum Command : uint
{
    None = 0,
    SendMessage = 1,
    SilentChat = 2,
    SubscribeChat = 4,
    AddTeacherToken = 8,
    RemoveTeacherToken = 16,
    AddAdminToken = 32,
    RemoveAdminToken = 64,
    ShowAllTokens = 128,
    All = ~None,
}