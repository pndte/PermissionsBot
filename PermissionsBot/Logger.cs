namespace PermissionsBot.Logger;

public class Logger
{
    public void Log(string text)
    {
        Console.WriteLine($"DEFAULT: {text}");
    }

    public void LogError(string text) // TODO: сделать имя пользователя в логах.
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"ERROR: {text}");
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public void LogWarning(string text)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"WARNING: {text}");
        Console.ForegroundColor = ConsoleColor.Gray;
    }
}