namespace CIService
{
    using System;
    using System.Collections.Generic;

    public class CiTestConfiguration
    {
        public CiTestConfiguration()
        {
            TestProjects=new List<string>();
        }
        public string SolutionPath { get; set; }
        public TimeSpan MinutesToWait { get; set; }
        public List<string> TestProjects { get; set; }

    }
}