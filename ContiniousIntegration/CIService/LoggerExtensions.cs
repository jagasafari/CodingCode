namespace Microsoft.Framework.Logging
{
    using System;

    public static class LoggerExtensions
    {
        public static void Info(this ILogger logger, string message)
        {
            logger.LogInformation(FormatLogMessage(message));
        }

        private static string FormatLogMessage(string message)
        {
            return $"[{DateTime.UtcNow}] :    {message}";
        }
    }
}