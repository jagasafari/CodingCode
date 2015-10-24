namespace CodingCode.Common
{
    using System;
    using System.Diagnostics;
    using System.Text;

    public class ProcessExecutor : IDisposable
    {
        private readonly StringBuilder _sb;

        public ProcessExecutor()
        {
            _sb = new StringBuilder();
            _processInstance = new Process();
        }

        public int ExitCode => _processInstance.ExitCode;
        public string Output => _sb.ToString();
        public void Dispose()
        {
            if(ExpectedExit) return;
            _processInstance.Kill();
        }

        public bool ExpectedExit { get; set; }

        private readonly Process _processInstance;

        private void ExecuteAndWait(string fileName, string arguments)
        {
            Execute(fileName, arguments, 10);
            _processInstance.WaitForExit();
        }

        public void Execute(string fileName, string arguments,
            int sleepTime)
        {
            _processInstance.StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            _processInstance.OutputDataReceived +=
                (sender, e) => { _sb.AppendLine(e.Data); };
            _processInstance.ErrorDataReceived +=
                (sender, e) => { _sb.AppendLine(e.Data); };
            _processInstance.Exited += (sender, e) => { };
            _processInstance.EnableRaisingEvents = true;

            _processInstance.Start();
            _processInstance.StandardInput.Dispose();
            _processInstance.BeginOutputReadLine();
            _processInstance.BeginErrorReadLine();
        }

        public void ExecuteAndWait(string programPath,
            string arguments,
            Func<string, bool> failurePredicate)
        {
            ExecuteAndWait(
                    programPath,
                    $"{arguments}");

            if (ExitCode != 0)
                throw new Exception(
                    $"Execution of {nameof(_processInstance)}: {programPath} with arguments:" +
                    $" {arguments} hasn't completed. {Output}");
            if (failurePredicate(Output))
                throw new Exception(
                    $"Command {programPath} failed {Output}");
        }
    }
}