namespace OneDriveLink.Processors;

using OneDriveLink.Helpers;

public static class Dispatcher
{
    public static async Task ExecuteAsync(string? inputUrl, bool isArgumentMode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(inputUrl))
            {
                Logger.LogInfo("No URL entered.", isArgumentMode);
                return;
            }

            if (!Uri.TryCreate(inputUrl, UriKind.Absolute, out var initialUri))
            {
                Logger.LogInfo("Invalid URL entered.", isArgumentMode);
                return;
            }

            if (initialUri.Host.Equals("1drv.ms", StringComparison.OrdinalIgnoreCase))
            {
                await Resolver.ProcessAsync(initialUri, isArgumentMode);
            }
            else
            {
                Logger.LogError("The provided URL does not match known OneDrive patterns.", isArgumentMode);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"An error occurred while processing the URL: {inputUrl}. Error: {ex.Message}", isArgumentMode);
        }
    }
}
