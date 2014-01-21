using Raven.TestSuite.Common.WrapperInterfaces;
using System;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class DomainContainer : IDisposable
    {
        private AppDomain domain;

        private Wrapper wrapperProxy;

        public DomainContainer(string clientDllPath, string version, int databasePort)
        {
            var clientDllFolder = System.IO.Path.GetDirectoryName(clientDllPath);
            var testSuiteRunningFolder = AppDomain.CurrentDomain.BaseDirectory;
            var setup = new AppDomainSetup();
            setup.ApplicationBase = clientDllFolder;
            domain = AppDomain.CreateDomain(version, null, setup);
            Type loaderType = typeof(Wrapper);
            wrapperProxy = (Wrapper)domain.CreateInstanceFrom(loaderType.Assembly.Location, loaderType.FullName).Unwrap();
            wrapperProxy.LoadAssemblyAndSetUp(clientDllPath, testSuiteRunningFolder, databasePort);
        }

        public IRavenClientWrapper Wrapper
        {
            get { return this.wrapperProxy; }
        }

        public void Dispose()
        {
            AppDomain.Unload(domain);
        }
    }
}
