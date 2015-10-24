namespace CodingCode.Common
{
    using System.Diagnostics;
    using System.IO;

    public static class DnxTool
    {
        public static string GetDnxPath()
        {
            return Path.GetDirectoryName(Process.GetCurrentProcess().Modules[0].FileName);
        }

        public static string GetDnx()
        {
            return Path.Combine(GetDnxPath(), "dnx.exe");
        }

        public static string GetDnu()
        {
            return Path.Combine(GetDnxPath(), "dnu.cmd");
        }
    }
}
