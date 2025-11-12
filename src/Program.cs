using OneDriveLink.Helpers;
using OneDriveLink.Processors;

namespace OneDriveLink
{
    class Program
    {
        static async Task Main(string[] args)
        {
            bool isArgumentMode = args.Length > 0;

            if (isArgumentMode)
            {
                foreach (var url in args)
                {
                    await Dispatcher.ExecuteAsync(url, true);
                }
            }
            else if (!Console.IsInputRedirected)
            {
                Console.Write("Please enter OneDrive URL: ");
                var inputUrl = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(inputUrl))
                    await Dispatcher.ExecuteAsync(inputUrl, false);

                Logger.LogInfo("Press any key to exit...", false);
                Console.ReadKey();
            }
            else
            {
                string? input;
                while ((input = Console.ReadLine()) != null)
                {
                    var trimmed = input.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                        await Dispatcher.ExecuteAsync(trimmed, false);
                }

                Logger.LogInfo("Press any key to exit...", false);
                Console.ReadKey();
            }
        }
    }
}
