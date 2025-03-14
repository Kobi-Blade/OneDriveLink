using System;
using System.Threading.Tasks;

namespace OneDriveLinkResolver
{
    class Program
    {
        static async Task Main()
        {
            Console.Write("Please enter shared URL (OneDrive/SharePoint): ");
            string? inputUrl = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(inputUrl))
            {
                Console.WriteLine("No URL entered. Exiting.");
                return;
            }

            if (!Uri.TryCreate(inputUrl, UriKind.Absolute, out Uri? initialUri))
            {
                Console.WriteLine("Invalid URL entered. Exiting.");
                return;
            }

            if (LinkTypeHelper.IsSharePointLink(initialUri))
            {
                await SharePointLinkProcessor.ProcessSharePointUrlAsync(initialUri);
            }
            else if (LinkTypeHelper.IsOneDriveLink(initialUri))
            {
                string encodedUrl = OneDriveUrlExtractor.ExtractEncodedUrl(initialUri);
                if (string.IsNullOrEmpty(encodedUrl))
                {
                    Console.WriteLine("No valid share identifier found in the URL.");
                    return;
                }
                string apiUrl = $"https://api.onedrive.com/v1.0/shares/{encodedUrl}/root/content";
                Console.WriteLine("API URL: " + apiUrl);

                string downloadUrl = await RedirectUrlProcessor.GetDownloadUrlAsync(initialUri);
                if (!string.IsNullOrEmpty(downloadUrl))
                {
                    Console.WriteLine("Download URL: " + downloadUrl);
                }
            }
            else
            {
                Console.WriteLine("The provided URL does not match known OneDrive or SharePoint patterns.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
