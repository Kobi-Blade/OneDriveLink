using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace OneDriveLinkResolver
{
    public static class SharePointLinkProcessor
    {
        private static readonly Uri ApiEntryPoint = new Uri("https://api.onedrive.com/v1.0/drives/");
        private static readonly Uri PersonalApiEntryPoint = new Uri("https://my.microsoftpersonalcontent.com/_api/v2.0/shares/");
        private static readonly Uri BadgerUrl = new Uri("https://api-badgerp.svc.ms/v1.0/token");
        private const string AppId = "1141147648";
        private const string AppUuid = "5cbed6ac-a083-4e14-b191-b4ba07653de2";

        public static async Task ProcessSharePointUrlAsync(Uri url)
        {
            Uri finalUrl = await FollowRedirectsAsync(url);
            Console.WriteLine("Resolved URL after redirection: " + finalUrl);

            SharePointAccessDetails accessDetails = SharePointAccessDetails.FromUri(finalUrl);

            using HttpClient client = new HttpClient();

            if (!string.IsNullOrEmpty(accessDetails.Redeem))
            {
                await GetBadgerTokenAsync(client);
            }

            Uri apiUrl = CreateApiUrl(accessDetails);
            Console.WriteLine("Constructed API URL: " + apiUrl);

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error accessing API URL: " + ex.Message);
                return;
            }

            JsonElement jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (!jsonResponse.TryGetProperty("name", out JsonElement fileNameProperty) ||
                !jsonResponse.TryGetProperty("@content.downloadUrl", out JsonElement downloadUrlProperty))
            {
                Console.WriteLine("Unexpected response from API.");
                return;
            }
            string fileName = fileNameProperty.GetString() ?? "downloaded_file";
            string downloadUrl = downloadUrlProperty.GetString() ?? string.Empty;

            Console.WriteLine("Resolved file name: " + fileName);
            Console.WriteLine("Download URL: " + downloadUrl);
        }

        /// <summary>
        /// Follows redirects for the provided URL and returns the final resolved URI.
        /// </summary>
        private static async Task<Uri> FollowRedirectsAsync(Uri url)
        {
            using HttpClient client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true });
            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to follow redirects: " + ex.Message);
                return url;
            }
            return response.RequestMessage?.RequestUri ?? url;
        }

        private static Uri CreateApiUrl(SharePointAccessDetails details)
        {
            if (!string.IsNullOrEmpty(details.Redeem))
            {
                string path = $"u!{details.Redeem}/driveitem";
                return new Uri(PersonalApiEntryPoint, path);
            }
            Uri baseUri = new Uri(ApiEntryPoint, $"{details.ContainerId}/items/{details.Resid}");
            if (!string.IsNullOrEmpty(details.AuthKey))
            {
                var query = new System.Collections.Generic.Dictionary<string, string?> { { "authkey", details.AuthKey } };
                string urlWithQuery = QueryHelpers.AddQueryString(baseUri.ToString(), query);
                return new Uri(urlWithQuery);
            }
            return baseUri;
        }

        private static async Task GetBadgerTokenAsync(HttpClient client)
        {
            client.DefaultRequestHeaders.Remove("AppId");
            client.DefaultRequestHeaders.Add("AppId", AppId);
            var data = new { appId = AppUuid };

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsJsonAsync(BadgerUrl, data);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Error retrieving token: " + ex.Message);
                return;
            }

            JsonElement jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (!jsonResponse.TryGetProperty("token", out JsonElement tokenElement))
            {
                Console.WriteLine("Token not found in response.");
                return;
            }
            string token = tokenElement.GetString() ?? "";

            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", $"Badger {token}");
            client.DefaultRequestHeaders.Remove("Prefer");
            client.DefaultRequestHeaders.Add("Prefer", "autoredeem");
        }
    }
}
