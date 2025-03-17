using System;

namespace OneDriveLink.Helpers
{
    public static class OneDriveUrlExtractor
    {
        /// <summary>
        /// Extracts the encoded share identifier (starting with "s!") from the URL segments.
        /// </summary>
        /// <param name="uri">The OneDrive shared URL.</param>
        /// <returns>The encoded share identifier if found; otherwise, an empty string.</returns>
        public static string ExtractEncodedUrl(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            foreach (var segment in uri.Segments)
            {
                var trimmedSegment = segment.Trim('/');
                if (trimmedSegment.StartsWith("s!", StringComparison.OrdinalIgnoreCase))
                {
                    return trimmedSegment;
                }
            }

            return string.Empty;
        }
    }
}