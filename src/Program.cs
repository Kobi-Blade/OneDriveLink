using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

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

        string encodedURL = string.Empty;
        foreach (string segment in initialUri.Segments)
        {
            string trimmedSegment = segment.Trim('/');
            if (trimmedSegment.StartsWith("s!"))
            {
                encodedURL = trimmedSegment;
                break;
            }
        }

        if (string.IsNullOrEmpty(encodedURL))
        {
            Console.WriteLine("No valid share identifier found in the URL.");
            return;
        }

        string apiUrl = $"https://api.onedrive.com/v1.0/shares/{encodedURL}/root/content";
        Console.WriteLine("API URL: " + apiUrl);

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

        var parsedQuery = QueryHelpers.ParseQuery(redirectUri.Query);
        string rebuiltQuery = string.Join("&",
            parsedQuery.SelectMany(kvp =>
                kvp.Value.Select(value => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(value)}")));
        uriBuilder.Query = rebuiltQuery;

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