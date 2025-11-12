using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace OneDriveLink.Models
{
    public record AccessInfo(string? ContainerId, string? Resid, string? AuthKey, string? Redeem)
    {
        public static AccessInfo FromUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            var query = QueryHelpers.ParseQuery(uri.Query);

            string? resid = GetQueryValue(query, "resid");
            string? redeem = GetQueryValue(query, "redeem");
            string? authKey = GetQueryValue(query, "authkey");
            string? idParam = GetQueryValue(query, "id");
            string? container = GetQueryValue(query, "cid");

            if (string.IsNullOrEmpty(resid) && !string.IsNullOrEmpty(idParam) && idParam.Contains('!'))
            {
                resid = idParam;
            }

            if (string.IsNullOrEmpty(container) && !string.IsNullOrEmpty(resid))
            {
                container = resid.Split('!')[0];
            }

            return new AccessInfo(container, resid, authKey, redeem);
        }

        private static string? GetQueryValue(Dictionary<string, StringValues> query, string key)
        {
            return query.TryGetValue(key, out var value) ? value.ToString() : null;
        }
    }
}