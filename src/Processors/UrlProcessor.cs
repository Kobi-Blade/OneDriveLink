using System;
using System.Threading.Tasks;
using OneDriveLink.Helpers;

namespace OneDriveLink.Processors
{
    public static class UrlProcessor
    {
        /// <summary>
        /// Processes the provided URL by identifying whether it's a SharePoint or OneDrive link,
        /// and handling it accordingly.
        /// </summary>
        public static async Task ProcessUrl(string? inputUrl, bool isArgumentMode)
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

                if (LinkTypeHelper.IsSharePointLink(initialUri))
                {
                    await SharePointLinkProcessor.ProcessSharePointUrlAsync(initialUri, isArgumentMode);
                }
                else if (LinkTypeHelper.IsOneDriveLink(initialUri))
                {
                    string encodedUrl = OneDriveUrlExtractor.ExtractEncodedUrl(initialUri);
                    if (string.IsNullOrEmpty(encodedUrl))
                    {
                        Logger.LogInfo("No valid share identifier found in the URL.", isArgumentMode);
                        return;
                    }
                    string apiUrl = $"https://api.onedrive.com/v1.0/shares/{encodedUrl}/root/content";
                    Logger.LogInfo("API URL: " + apiUrl, isArgumentMode);

                    string downloadUrl = await RedirectUrlProcessor.GetDownloadUrlAsync(initialUri);
                    if (!string.IsNullOrEmpty(downloadUrl))
                    {
                        Logger.LogUrl(downloadUrl, isArgumentMode);
                    }
                    else
                    {
                        Logger.LogError("Failed to resolve download URL for: " + initialUri, isArgumentMode);
                    }
                }
                else
                {
                    Logger.LogError("The provided URL does not match known OneDrive or SharePoint patterns.", isArgumentMode);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"An error occurred while processing the URL: {inputUrl}. Error: {ex.Message}", isArgumentMode);
            }
        }
    }
}