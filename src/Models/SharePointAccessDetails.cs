using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace OneDriveLink.Models
{
    /// <summary>
    /// Represents access details for a SharePoint resource.
    /// </summary>
    public record SharePointAccessDetails(string? ContainerId, string? Resid, string? AuthKey, string? Redeem)
    {
        /// <summary>
        /// Creates an instance of <see cref="SharePointAccessDetails"/> from a given URI.
        /// </summary>
        /// <param name="uri">The URI containing SharePoint access parameters.</param>
        /// <returns>An instance of <see cref="SharePointAccessDetails"/> populated with data from the URI.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided URI is null.</exception>
        public static SharePointAccessDetails FromUri(Uri uri)
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

            return new SharePointAccessDetails(container, resid, authKey, redeem);
        }

        /// <summary>
        /// Retrieves the value associated with the specified key from the query dictionary.
        /// </summary>
        /// <param name="query">The dictionary containing query parameters.</param>
        /// <param name="key">The key whose value is to be retrieved.</param>
        /// <returns>The value associated with the key if present; otherwise, null.</returns>
        private static string? GetQueryValue(Dictionary<string, StringValues> query, string key)
        {
            return query.TryGetValue(key, out var value) ? value.ToString() : null;
        }
    }
}