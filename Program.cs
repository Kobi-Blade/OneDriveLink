using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

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

        using HttpClient client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(initialUri);
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine("Error sending request: " + httpEx.Message);
            return;
        }

        if (!IsRedirect(response.StatusCode))
        {
            Console.WriteLine($"No redirect detected. Status code: {response.StatusCode}");
            return;
        }

        if (response.Headers.Location == null)
        {
            Console.WriteLine("Redirect location not provided by the response.");
            return;
        }

        Uri redirectUri = response.Headers.Location;

        UriBuilder uriBuilder = new UriBuilder(redirectUri)
        {
            Path = redirectUri.AbsolutePath.Replace("redir", "download")
        };

        var queryParams = HttpUtility.ParseQueryString(redirectUri.Query);
        uriBuilder.Query = queryParams.ToString();

        if (uriBuilder.Port == 443)
        {
            uriBuilder.Port = -1;
        }

        string downloadUrl = uriBuilder.ToString();
        Console.WriteLine("Download URL: " + downloadUrl);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static bool IsRedirect(HttpStatusCode statusCode) =>
        statusCode == HttpStatusCode.Redirect ||
        statusCode == HttpStatusCode.Moved ||
        statusCode == HttpStatusCode.MovedPermanently;
}
