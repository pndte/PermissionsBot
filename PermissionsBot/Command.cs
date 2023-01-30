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
    MakeSilent = 8,
    Subscribe = 16,
    Unsubscribe = 32,
    CreateTeacherToken = 64,
    CreateAdminToken = 128,
    RemoveToken = 256,
    ShowAllTokens = 512,
    All = ~None,
}