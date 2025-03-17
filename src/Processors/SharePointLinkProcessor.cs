using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using OneDriveLink.Helpers;
using OneDriveLink.Models;

namespace OneDriveLink.Processors
{
    public static class SharePointLinkProcessor
    {
        private static readonly Uri ApiEntryPoint = new Uri("https://api.onedrive.com/v1.0/drives/");
        private static readonly Uri PersonalApiEntryPoint = new Uri("https://my.microsoftpersonalcontent.com/_api/v2.0/shares/");
        private static readonly Uri BadgerUrl = new Uri("https://api-badgerp.svc.ms/v1.0/token");
        private const string AppId = "1141147648";
        private const string AppUuid = "5cbed6ac-a083-4e14-b191-b4ba07653de2";

        /// <summary>
        /// Resolves the SharePoint URL by following redirects and processing the response.
        /// </summary>
        public static async Task ProcessSharePointUrlAsync(Uri url, bool isArgumentMode = false)
        {
            try
            {
                var finalUrl = await FollowRedirectsAsync(url, isArgumentMode);
                var accessDetails = SharePointAccessDetails.FromUri(finalUrl);
                using var client = new HttpClient();

                if (!string.IsNullOrEmpty(accessDetails.Redeem))
                {
                    await GetBadgerTokenAsync(client, isArgumentMode);
                }

                var apiUrl = CreateApiUrl(accessDetails);
                Logger.LogInfo($"API URL: {apiUrl}", isArgumentMode);

                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (!jsonResponse.TryGetProperty("name", out var fileNameProperty) ||
                    !jsonResponse.TryGetProperty("@content.downloadUrl", out var downloadUrlProperty))
                {
                    Logger.LogError("Unexpected response from API.", isArgumentMode);
                    return;
                }

                var fileName = fileNameProperty.GetString() ?? "downloaded_file";
                var downloadUrl = downloadUrlProperty.GetString() ?? string.Empty;

                Logger.LogUrl(downloadUrl, isArgumentMode);
				
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error processing SharePoint URL: {url}. Error: {ex.Message}", isArgumentMode);
            }
        }

        /// <summary>
        /// Follows redirects for the provided URL and returns the final resolved URI.
        /// </summary>
        private static async Task<Uri> FollowRedirectsAsync(Uri url, bool isArgumentMode)
        {
            using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true });
            HttpResponseMessage response;

            try
            {
                response = await client.GetAsync(url);
            }
            catch (Exception ex)
            {
				Logger.LogError($"Failed to follow redirects: {ex.Message}", isArgumentMode);
                return url;
            }

            return response.RequestMessage?.RequestUri ?? url;
        }

        private static Uri CreateApiUrl(SharePointAccessDetails details)
        {
            if (!string.IsNullOrEmpty(details.Redeem))
            {
                var path = $"u!{details.Redeem}/driveitem";
                return new Uri(PersonalApiEntryPoint, path);
            }

            var baseUri = new Uri(ApiEntryPoint, $"{details.ContainerId}/items/{details.Resid}");

            if (!string.IsNullOrEmpty(details.AuthKey))
            {
                var query = new System.Collections.Generic.Dictionary<string, string?> { { "authkey", details.AuthKey } };
                var urlWithQuery = QueryHelpers.AddQueryString(baseUri.ToString(), query);
                return new Uri(urlWithQuery);
            }

            return baseUri;
        }

        /// <summary>
        /// Retrieves the Badger token for authentication.
        /// </summary>
        private static async Task GetBadgerTokenAsync(HttpClient client, bool isArgumentMode)
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
                Logger.LogError($"Error retrieving token: {ex.Message}", isArgumentMode);
                return;
            }

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (!jsonResponse.TryGetProperty("token", out var tokenElement))
            {
                Logger.LogError("Token not found in response.", isArgumentMode);
                return;
            }

            var token = tokenElement.GetString() ?? string.Empty;
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", $"Badger {token}");
            client.DefaultRequestHeaders.Remove("Prefer");
            client.DefaultRequestHeaders.Add("Prefer", "autoredeem");
        }
    }
}