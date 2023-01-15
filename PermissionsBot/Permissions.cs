using Telegram.Bot;

namespace PermissionsBot;

[Flags]
public enum Permissions : uint
{
    none = 0,

    read_message = 1,
    write_message = 2,
    delete_message = 4,

    pin_message = 8,

    record_voice_message = 16,

    add_user = 32,
    kick_user = 64,
    mute_user = 128,
    
    all = ~none
}