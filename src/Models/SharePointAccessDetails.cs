using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace OneDriveLinkResolver
{
    public record SharePointAccessDetails(string ContainerId, string Resid, string AuthKey, string Redeem)
    {
        public static SharePointAccessDetails FromUri(Uri uri)
        {
            Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(uri.Query);

            string resid = GetQueryValue(query, "resid");
            string redeem = GetQueryValue(query, "redeem");
            string authKey = GetQueryValue(query, "authkey");
            string idParam = GetQueryValue(query, "id");
            string? container = GetQueryValue(query, "cid");

            if (string.IsNullOrEmpty(resid) && !string.IsNullOrEmpty(idParam) && idParam.Contains("!"))
            {
                resid = idParam;
            }

            if (string.IsNullOrEmpty(container) && !string.IsNullOrEmpty(resid))
            {
                container = resid.Split('!')[0];
            }

            return new SharePointAccessDetails(container, resid, authKey, redeem);
        }

        private static string GetQueryValue(Dictionary<string, StringValues> query, string key)
        {
            if (query.TryGetValue(key, out StringValues value))
            {
                return value.ToString();
            }
            return string.Empty;
        }
    }
}
