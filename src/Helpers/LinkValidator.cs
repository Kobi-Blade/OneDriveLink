namespace OneDriveLink.Helpers
{
    public static class LinkValidator
    {
        public static bool IsValidLink(Uri uri)
        {
            return uri != null &&
                   uri.Host.Equals("1drv.ms", StringComparison.OrdinalIgnoreCase);
        }
    }
}