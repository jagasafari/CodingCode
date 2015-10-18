namespace CodingCode.Common.Contracts
{
    using System.Diagnostics;

    public interface IDnxProcess
    {
        bool ExpectedExit { get; set; }
        Process ProcessInstance { get; set; }
        string ExecuteAndWait( string programPath, string arguments );
        void Execute( string fileName, string arguments,int sleepTime );
    }
}