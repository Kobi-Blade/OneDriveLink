using System;
using System.Threading.Tasks;

namespace OneDriveLinkResolver
{
    class Program
    {
        static async Task Main()
        {
            Console.Write("Please enter shared OneDrive URL: ");
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

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
