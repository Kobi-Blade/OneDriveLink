using System;
using System.Threading.Tasks;

namespace OneDriveLinkResolver
{
    class Program
    {
        private static bool isArgumentMode = false;

        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                isArgumentMode = true;
                foreach (var url in args)
                {
                    await ProcessUrl(url);
                }
            }
            else if (!Console.IsInputRedirected)
            {
                Console.Write("Please enter shared URL (OneDrive/SharePoint): ");
                string? inputUrl = Console.ReadLine()?.Trim();
                await ProcessUrl(inputUrl);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            else
            {
                string? input;
                while ((input = Console.ReadLine()) != null)
                {
                    await ProcessUrl(input.Trim());
                }
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static async Task ProcessUrl(string? inputUrl)
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
                            Console.WriteLine(downloadUrl); // Echo the download URL
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