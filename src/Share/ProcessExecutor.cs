namespace Share
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;
    using Contracts;

    public class ProcessExecutor : IDnxProcess, IDisposable
    {
        private readonly StringBuilder _sb;

        public ProcessExecutor()
        {
            _sb = new StringBuilder();
        }

        public int ExitCode => ProcessInstance.ExitCode;

        public void Dispose()
        {
            if(ExpectedExit) return;
            ProcessInstance.Kill();
        }

        public bool ExpectedExit { get; set; }

        public Process ProcessInstance { get; set; }

        public string ExecuteAndWait(string fileName, string arguments)
        {
            Execute(fileName, arguments, 10);
            ProcessInstance.WaitForExit();
            return _sb.ToString();
        }

        public void Execute(string fileName, string arguments,
            int sleepTime)
        {
            ProcessInstance.StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            ProcessInstance.OutputDataReceived +=
                (sender, e) => { _sb.AppendLine(e.Data); };
            ProcessInstance.ErrorDataReceived +=
                (sender, e) => { _sb.AppendLine(e.Data); };
            ProcessInstance.Exited += (sender, e) => { };
            ProcessInstance.EnableRaisingEvents = true;

            ProcessInstance.Start();
            ProcessInstance.StandardInput.Dispose();
            ProcessInstance.BeginOutputReadLine();
            ProcessInstance.BeginErrorReadLine();
        }

        public static void ExecuteAndWait(string programPath, string arguments,
            Func<string, bool> failurePredicate)
        {
            var dnxProcess = new ProcessExecutor
            {
                ProcessInstance = new Process(),
                ExpectedExit = true
            };
            var result =
                dnxProcess.ExecuteAndWait(
                    programPath,
                    $"{arguments}");

            if(dnxProcess.ExitCode != 0)
                throw new Exception(
                    $"Execution of {nameof(dnxProcess)}: {programPath} with arguments:" +
                    $" {arguments} hasn't completed. {result}");
            if(failurePredicate(result))
                throw new Exception($"Command {programPath} failed {result}");
        }

        public static void ExecuteInShellAndWait(string dnxProcessPath, string arguments)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = dnxProcessPath,
                Arguments = arguments
            };
            var process = new ProcessExecutor
            {
                ExpectedExit = true,
                ProcessInstance = new Process {StartInfo = processStartInfo}
            };
            process.ProcessInstance.Start();
            process.ProcessInstance.WaitForExit();
        }
    }
}