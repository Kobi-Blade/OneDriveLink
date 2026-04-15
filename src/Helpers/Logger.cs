namespace OneDriveLink.Helpers;

public static class Logger
{
    public static void LogInfo(string message, bool isArgumentMode)
    {
        if (!isArgumentMode)
        {
            Console.WriteLine(message);
        }
    }

    public static void LogError(string message, bool isArgumentMode)
    {
        if (isArgumentMode)
        {
            Console.Error.WriteLine(message);
        }
        else
        {
            Console.WriteLine(message);
        }
    }

    public static void LogUrl(string message, bool isArgumentMode) =>
        Console.WriteLine(isArgumentMode ? message : $"Download URL: {message}");
}
