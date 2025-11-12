using OneDriveLink.Helpers;

namespace OneDriveLink.Processors
{
    public static class Dispatcher
    {
        public static async Task ExecuteAsync(string? inputUrl, bool isArgumentMode)
        {
            try
            {
                if (string.IsNullOrEmpty(inputUrl))
                {
                    Logger.LogInfo("No URL entered.", isArgumentMode);
                    return;
                }

                if (!Uri.TryCreate(inputUrl, UriKind.Absolute, out Uri? initialUri))
                {
                    Logger.LogInfo("Invalid URL entered.", isArgumentMode);
                    return;
                }

                if (LinkValidator.IsValidLink(initialUri))
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
}