namespace Share
{
    using System.Diagnostics;
    using System.IO;

    public static class DnxTool
    {
        public static string GetDnxPath()
        {
            return Path.GetDirectoryName(Process.GetCurrentProcess().Modules[0].FileName);
        }
    }
}
