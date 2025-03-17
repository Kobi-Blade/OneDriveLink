using System;

namespace OneDriveLink.Helpers
{
    public static class Logger
    {
        /// <summary>
        /// Logs an informational message. If in argument mode, logs nothing to avoid noise.
        /// </summary>
        public static void LogInfo(string message, bool isArgumentMode)
        {
            if (!isArgumentMode)
                Console.WriteLine(message);
        }

        /// <summary>
        /// Logs an error message. If in argument mode, logs to the standard error stream.
        /// </summary>
        public static void LogError(string message, bool isArgumentMode)
        {
            if (!isArgumentMode)
                Console.WriteLine(message);
            else
                Console.Error.WriteLine(message);
        }

        /// <summary>
        /// Logs download url.
        /// </summary>
        public static void LogUrl(string message, bool isArgumentMode)
        {
            if (!isArgumentMode)
                Console.WriteLine("Download URL: " + message);
            else
                Console.WriteLine(message);
        }
    }
}
