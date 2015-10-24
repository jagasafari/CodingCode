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
    }
}