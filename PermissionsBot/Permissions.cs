using Telegram.Bot;

namespace PermissionsBot;

[Flags]
public enum Permissions : uint
{
    None = 0,

    ReadMessage = 1,
    WriteMessage = 2,
    DeleteMessage = 4,

    PinMassage = 8,

    RecordVoiceMessage = 16,

    AddUser = 32,
    KickUser = 64,
    MuteUser = 128,
    
    All = ~None
}