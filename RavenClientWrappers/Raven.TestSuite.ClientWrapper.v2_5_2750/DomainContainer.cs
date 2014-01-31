using System;
using System.IO;
using Raven.TestSuite.Common.Abstractions;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class DomainContainer : IDomainContainer
    {
        private AppDomain domain;

        private Wrapper wrapperProxy;

        public DomainContainer(string ravenVersionFolderPath, string domainName, int databasePort)
        {
            var clientDllPath = Path.Combine(ravenVersionFolderPath, Constants.Paths.ClientDllPartialPath);
            var clientDllFolder = System.IO.Path.GetDirectoryName(clientDllPath);
            var testSuiteRunningFolder = AppDomain.CurrentDomain.BaseDirectory;
            var setup = new AppDomainSetup();
            setup.ApplicationBase = clientDllFolder;
            domain = AppDomain.CreateDomain(domainName, null, setup);
            Type loaderType = typeof(Wrapper);
            wrapperProxy = (Wrapper)domain.CreateInstanceFrom(loaderType.Assembly.Location, loaderType.FullName).Unwrap();
            wrapperProxy.LoadAssemblyAndSetUp(clientDllPath, ravenVersionFolderPath, testSuiteRunningFolder, databasePort);
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
