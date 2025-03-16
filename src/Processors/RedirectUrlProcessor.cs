using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace OneDriveLink.Processors
{
    public static class RedirectUrlProcessor
    {
        /// <summary>
        /// Sends an HTTP GET request to the initial URL, checks for a redirect, and processes it into a download URL.
        /// </summary>
        /// <param name="initialUri">The original OneDrive URL provided by the user.</param>
        /// <returns>The processed download URL if successful; otherwise, an empty string.</returns>
        public static async Task<string> GetDownloadUrlAsync(Uri initialUri)
        {
            using HttpClient client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
            HttpResponseMessage response;

            try
            {
                response = await client.GetAsync(initialUri);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error sending request: " + ex.Message);
                return string.Empty;
            }

            if (!IsRedirect(response.StatusCode))
            {
                Console.WriteLine($"No redirect detected. Status code: {response.StatusCode}");
                return string.Empty;
            }

            if (response.Headers.Location == null)
            {
                Console.WriteLine("Redirect location not provided by the response.");
                return string.Empty;
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

            return uriBuilder.ToString();
        }

        private static bool IsRedirect(HttpStatusCode statusCode) =>
            statusCode == HttpStatusCode.Redirect ||
            statusCode == HttpStatusCode.Moved ||
            statusCode == HttpStatusCode.MovedPermanently;
    }
}
