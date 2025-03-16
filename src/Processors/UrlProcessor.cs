using System;
using System.Threading.Tasks;
using OneDriveLink.Helpers;

namespace OneDriveLink.Processors
{
    public static class UrlProcessor
    {
        public static async Task ProcessUrl(string? inputUrl, bool isArgumentMode)
        {
            try
            {
                if (string.IsNullOrEmpty(inputUrl))
                {
                    if (!isArgumentMode) Console.WriteLine("No URL entered.");
                    return;
                }

                if (!Uri.TryCreate(inputUrl, UriKind.Absolute, out Uri? initialUri))
                {
                    if (!isArgumentMode) Console.WriteLine("Invalid URL entered.");
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
                        if (!isArgumentMode) Console.WriteLine("No valid share identifier found in the URL.");
                        return;
                    }
                    string apiUrl = $"https://api.onedrive.com/v1.0/shares/{encodedUrl}/root/content";
                    if (!isArgumentMode) Console.WriteLine("API URL: " + apiUrl);

                    string downloadUrl = await RedirectUrlProcessor.GetDownloadUrlAsync(initialUri);
                    if (!string.IsNullOrEmpty(downloadUrl))
                    {
                        if (!isArgumentMode)
                        {
                            Console.WriteLine("Download URL: " + downloadUrl);
                        }
                        else
                        {
                            Console.WriteLine(downloadUrl);
                        }
                    }
                    else
                    {
                        if (!isArgumentMode) Console.WriteLine("Failed to resolve download URL for: " + inputUrl);
                    }
                }
                else
                {
                    if (!isArgumentMode) Console.WriteLine("The provided URL does not match known OneDrive or SharePoint patterns.");
                }
            }
            catch (Exception ex)
            {
                if (!isArgumentMode)
                {
                    Console.WriteLine($"An error occurred while processing the URL: {inputUrl}. Error: {ex.Message}");
                }
                else
                {
                    Console.Error.WriteLine($"Error processing URL: {inputUrl}. Error: {ex.Message}");
                }
            }
        }
    }
}
