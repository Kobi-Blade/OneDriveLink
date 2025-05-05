using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using OneDriveLink.Helpers;

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
            if (initialUri == null)
                throw new ArgumentNullException(nameof(initialUri));

            using var handler = new HttpClientHandler { AllowAutoRedirect = false };
            using var client = new HttpClient(handler);

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(initialUri);
            }
            catch (HttpRequestException)
            {
                return string.Empty;
            }

            if (!IsRedirect(response.StatusCode))
            {
                return string.Empty;
            }

            var redirectUri = response.Headers.Location;
            if (redirectUri == null)
            {
                return string.Empty;
            }

            var uriBuilder = new UriBuilder(redirectUri)
            {
                Path = redirectUri.AbsolutePath.Replace("redir", "download")
            };

            var parsedQuery = QueryHelpers.ParseQuery(redirectUri.Query);
            var rebuiltQuery = string.Join("&",
                parsedQuery.SelectMany(kvp =>
                    kvp.Value.Select(value => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(value)}")));
            uriBuilder.Query = rebuiltQuery;

            if (uriBuilder.Port == 443)
            {
                uriBuilder.Port = -1;
            }

            return uriBuilder.ToString();
        }

        /// <summary>
        /// Determines if the provided status code indicates a redirect.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to evaluate.</param>
        /// <returns>True if the status code indicates a redirect; otherwise, false.</returns>
        private static bool IsRedirect(HttpStatusCode statusCode) =>
            statusCode == HttpStatusCode.Redirect ||
            statusCode == HttpStatusCode.Moved ||
            statusCode == HttpStatusCode.MovedPermanently;
    }
}