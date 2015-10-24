namespace CodingCode.Common
{
    using System.Diagnostics;

    public class InSellProcessExecutor
    {
        public static void ExecuteAndWait(string dnxProcessPath,
            string arguments)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = dnxProcessPath,
                Arguments = arguments
            };
            var process = new Process
            {
                StartInfo = processStartInfo
            };
            process.Start();
            process.WaitForExit();
        }
    }
}