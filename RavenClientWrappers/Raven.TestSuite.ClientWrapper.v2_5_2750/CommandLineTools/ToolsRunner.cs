using System.Diagnostics;
using Raven.TestSuite.Common.Abstractions;
using System.IO;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750.CommandLineTools
{
    public class ToolsRunner
    {
        private string smugglerExePath;

        public ToolsRunner(string testSuiteRunningFolder)
        {
            this.smugglerExePath = Path.Combine(testSuiteRunningFolder, Constants.Paths.SmugglerExePartialPath);
        }

        public void RunSmuggler(string arguments)
        {
            var smugglerProcess = new Process();
            smugglerProcess.StartInfo.FileName = this.smugglerExePath;
            smugglerProcess.StartInfo.Arguments = arguments;
            smugglerProcess.StartInfo.LoadUserProfile = false;
            smugglerProcess.StartInfo.UseShellExecute = false;
            smugglerProcess.StartInfo.RedirectStandardError = true;
            smugglerProcess.StartInfo.RedirectStandardInput = true;
            smugglerProcess.StartInfo.RedirectStandardOutput = true;
            smugglerProcess.StartInfo.CreateNoWindow = true;
            smugglerProcess.Start();
            smugglerProcess.WaitForExit();
        }
    }
}
