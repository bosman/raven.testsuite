using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using Raven.TestSuite.Common;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class DomainContainer : IDisposable
    {
        private AppDomain domain;

        private Wrapper wrapperProxy;

        public DomainContainer(string clientDllPath, string version, string appDomainPath, string originalPath)
        {
             var e = new Evidence(AppDomain.CurrentDomain.Evidence);
            var setup = new AppDomainSetup();
            setup.ApplicationBase = appDomainPath;
            domain = AppDomain.CreateDomain(version, null, setup);
            Type loaderType = typeof(Wrapper);
            wrapperProxy = (Wrapper)domain.CreateInstanceFrom(loaderType.Assembly.Location, loaderType.FullName).Unwrap();
            wrapperProxy.LoadDocumentStoreAndInitialize(clientDllPath);
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
