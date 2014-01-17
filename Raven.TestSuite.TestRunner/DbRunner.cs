using System;
using System.Diagnostics;

namespace Raven.TestSuite.TestRunner
{
    public class DbRunner : IDisposable
    {
        private bool _disposed;
        private Process dbServerProcess;

        public static DbRunner Run(int port, string standaloneServerExePath)
        {
            return new DbRunner(port, standaloneServerExePath);
        }

        private DbRunner(int port, string standaloneServerExePath)
        {
            System.Console.WriteLine("Starting test with database on port " + port);
            this.dbServerProcess = new Process();
            this.dbServerProcess.StartInfo.FileName = "C:\\RavenDB-Build-2750\\Server\\Raven.Server.exe";
            this.dbServerProcess.StartInfo.Arguments = "--set=Raven/Port==" + port;
            this.dbServerProcess.StartInfo.LoadUserProfile = false;
            this.dbServerProcess.StartInfo.UseShellExecute = false;
            this.dbServerProcess.StartInfo.RedirectStandardError = true;
            this.dbServerProcess.StartInfo.RedirectStandardInput = true;
            this.dbServerProcess.StartInfo.RedirectStandardOutput = true;
            this.dbServerProcess.StartInfo.CreateNoWindow = true;
            this.dbServerProcess.Start();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DbRunner()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
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
            }
            _disposed = true;
        }
    }
}
