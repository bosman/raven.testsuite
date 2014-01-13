using System;
using System.Collections.Generic;
using System.Diagnostics;
using Raven.TestSuite.Common;
using Raven.TestSuite.Tests;

namespace Raven.TestSuite.TestRunner
{
    public class TestTests
    {
        public List<TestResult> RunTest()
        {
            var dbPort = 8080;

            System.Console.WriteLine("Starting test with database on port " + dbPort);

            Process dbServerProcess = new Process();
            dbServerProcess.StartInfo.FileName = "C:\\RavenDB-Build-2750\\Server\\Raven.Server.exe";
            dbServerProcess.StartInfo.Arguments = "--set=Raven/Port==" + dbPort;
            dbServerProcess.StartInfo.LoadUserProfile = false;
            dbServerProcess.StartInfo.UseShellExecute = false;
            dbServerProcess.StartInfo.RedirectStandardError = true;
            dbServerProcess.StartInfo.RedirectStandardInput = true;
            dbServerProcess.StartInfo.RedirectStandardOutput = true;
            dbServerProcess.StartInfo.CreateNoWindow = true;
            dbServerProcess.Start();

            List<TestResult> testResults;

            using (var domainContainer = new ClientWrapper.v2_5_2750.DomainContainer(
                "C:\\RavenDB-Build-2750\\Client\\Raven.Client.Lightweight.dll", "version1", dbPort))
            {
                var wrapper = domainContainer.Wrapper;
                System.Console.WriteLine(wrapper.GetVersion());

                var someTestGroup = new InitialDevTests(wrapper);
                testResults = someTestGroup.RunTests();
            }

            try
            {
                dbServerProcess.StandardInput.Write("q\r\n");
            }
            catch (Exception)
            {
            }
            if (!dbServerProcess.WaitForExit(10000))
                throw new Exception("RavenDB command-line server did not halt within 10 seconds of pressing enter.");

            string errorOutput = dbServerProcess.StandardError.ReadToEnd();
            string output = dbServerProcess.StandardOutput.ReadToEnd();

            if (!String.IsNullOrEmpty(errorOutput))
                throw new Exception("RavendB command-line server finished with error text: " + errorOutput + "\r\n" + output);

            if (dbServerProcess.ExitCode != 0)
                throw new Exception("RavenDB command-line server finished with exit code: " + dbServerProcess.ExitCode + " " + output);

            
            dbServerProcess.Close();

            return testResults;
        }
    }
}
