using System;

namespace OneDriveLink.Helpers
{
    public static class LinkTypeHelper
    {
        /// <summary>
        /// Identifies a OneDrive link.
        /// Expected pattern: https://1drv.ms/u/s!...
        /// </summary>
        public static bool IsOneDriveLink(Uri uri)
        {
            if (!uri.Host.Equals("1drv.ms", StringComparison.OrdinalIgnoreCase))
                return false;

            var segments = uri.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 2)
                return false;

            if (!segments[0].Equals("u", StringComparison.OrdinalIgnoreCase))
                return false;

            return segments[1].StartsWith("s!", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Identifies a SharePoint link.
        /// Expected patterns are:
        ///   - https://1drv.ms/u/c/...
        ///   - https://1drv.ms/t/...
        ///   - https://1drv.ms/f/...
        /// </summary>
        public static bool IsSharePointLink(Uri uri)
        {
            if (!uri.Host.Equals("1drv.ms", StringComparison.OrdinalIgnoreCase))
                return false;

            var segments = uri.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 2)
                return false;

            string firstSegment = segments[0].ToLowerInvariant();

            if (firstSegment == "t" || firstSegment == "f")
                return true;

            if (firstSegment == "u")
                return segments[1].Equals("c", StringComparison.OrdinalIgnoreCase);

            return false;
        }
    }
}