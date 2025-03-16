using System;
using System.Threading.Tasks;
using OneDriveLink.Processors;

namespace OneDriveLink
{
    class Program
    {
        private static bool isArgumentMode = false;

        private static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                isArgumentMode = true;
                foreach (var url in args)
                {
                    await UrlProcessor.ProcessUrl(url, isArgumentMode);
                }
            }
            else if (!Console.IsInputRedirected)
            {
                Console.Write("Please enter shared URL (OneDrive/SharePoint): ");
                string? inputUrl = Console.ReadLine()?.Trim();
                await UrlProcessor.ProcessUrl(inputUrl, isArgumentMode);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            else
            {
                string? input;
                while ((input = Console.ReadLine()) != null)
                {
                    await UrlProcessor.ProcessUrl(input.Trim(), isArgumentMode);
                }
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}