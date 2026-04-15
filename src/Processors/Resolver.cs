namespace OneDriveLink.Processors;

using Microsoft.AspNetCore.WebUtilities;
using OneDriveLink.Helpers;
using OneDriveLink.Models;
using System.Net.Http.Json;
using System.Text.Json;

public static class Resolver
{
    private static readonly Uri ApiEntryPoint = new("https://api.onedrive.com/v1.0/drives/");
    private static readonly Uri PersonalApiEntryPoint = new("https://my.microsoftpersonalcontent.com/_api/v2.0/shares/");
    private static readonly Uri BadgerUrl = new("https://api-badgerp.svc.ms/v1.0/token");
    private const string AppId = "1141147648";
    private const string AppUuid = "5cbed6ac-a083-4e14-b191-b4ba07653de2";

    public static async Task ProcessAsync(Uri url, bool isArgumentMode = false)
    {
        try
        {
            using var followClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true });
            var followResponse = await followClient.GetAsync(url);
            var finalUrl = followResponse.RequestMessage?.RequestUri ?? url;

            var accessInfo = AccessInfo.FromUri(finalUrl);
            using var client = new HttpClient();

            if (!string.IsNullOrEmpty(accessInfo.Redeem))
            {
                client.DefaultRequestHeaders.Remove("AppId");
                client.DefaultRequestHeaders.Add("AppId", AppId);

                var tokenResponse = await client.PostAsJsonAsync(BadgerUrl, new { appId = AppUuid });
                tokenResponse.EnsureSuccessStatusCode();

                var tokenJson = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();

                if (!tokenJson.TryGetProperty("token", out var tokenElement))
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

            Uri apiUrl;
            if (!string.IsNullOrEmpty(accessInfo.Redeem))
            {
                apiUrl = new Uri(PersonalApiEntryPoint, $"u!{accessInfo.Redeem}/driveitem");
            }
            else
            {
                var baseUri = new Uri(ApiEntryPoint, $"{accessInfo.ContainerId}/items/{accessInfo.Resid}");
                if (!string.IsNullOrEmpty(accessInfo.AuthKey))
                {
                    var query = new Dictionary<string, string?> { ["authkey"] = accessInfo.AuthKey };
                    apiUrl = new Uri(QueryHelpers.AddQueryString(baseUri.ToString(), query));
                }
                else
                {
                    apiUrl = baseUri;
                }
            }

            Logger.LogInfo($"API URL: {apiUrl}", isArgumentMode);

            using var response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (!jsonResponse.TryGetProperty("name", out _) ||
                !jsonResponse.TryGetProperty("@content.downloadUrl", out var downloadUrlProperty))
            {
                Logger.LogError("Unexpected response from API.", isArgumentMode);
                return;
            }

            Logger.LogUrl(downloadUrlProperty.GetString() ?? string.Empty, isArgumentMode);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error processing URL: {url}. Error: {ex.Message}", isArgumentMode);
        }
    }
}
