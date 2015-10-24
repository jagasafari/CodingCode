namespace CIService
{
    using System.Collections.Generic;

    public class CiTestConfiguration
    {
        public CiTestConfiguration()
        {
            TestProjects=new List<string>();
        }
        public string SolutionPath { get; set; }
        public int MinutesToWait { get; set; }
        public List<string> TestProjects { get; set; }
        public string Sender { get; set; }
        public string Password { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpHost { get; set; }
        public string Receiver { get; set; }
    }
}