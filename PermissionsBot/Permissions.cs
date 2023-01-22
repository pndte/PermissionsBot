namespace PermissionsBot;

public static class Permissions
{
    public const Command TEACHER =
        Command.Register | Command.SendMessage | Command.SendMessageTo | Command.SilentChat | Command.SubscribeChat;

    public const Command ADMIN = Command.All;
}