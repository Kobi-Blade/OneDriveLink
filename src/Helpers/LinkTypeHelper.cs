using System;

namespace OneDriveLink.Helpers
{
    public static class LinkTypeHelper
    {
        /// <summary>
        /// Determines if a given URI is a OneDrive link.
        /// Expected pattern: https://1drv.ms/u/s!...
        /// </summary>
        public static bool IsOneDriveLink(Uri uri)
        {
            if (uri == null || !uri.Host.Equals("1drv.ms", StringComparison.OrdinalIgnoreCase))
                return false;

            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return segments.Length >= 2 &&
                   segments[0].Equals("u", StringComparison.OrdinalIgnoreCase) &&
                   segments[1].StartsWith("s!", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines if a given URI is a SharePoint link.
        /// Expected patterns include:
        ///   - https://1drv.ms/u/c/...
        ///   - https://1drv.ms/t/...
        ///   - https://1drv.ms/f/...
        /// </summary>
        public static bool IsSharePointLink(Uri uri)
        {
            if (uri == null || !uri.Host.Equals("1drv.ms", StringComparison.OrdinalIgnoreCase))
                return false;

            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 2)
                return false;

            var firstSegment = segments[0].ToLowerInvariant();
            return firstSegment switch
            {
                "t" => true,
                "f" => true,
                "u" => segments[1].Equals("c", StringComparison.OrdinalIgnoreCase),
                _ => false,
            };
        }
    }
}