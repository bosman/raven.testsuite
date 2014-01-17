using Raven.TestSuite.Common.WrapperInterfaces;
using System;
using System.IO;
using System.Reflection;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class Wrapper : MarshalByRefObject, IRavenClientWrapper, IDocumentStoreWrapper
    {
        private Assembly assembly;
        private Client.Document.DocumentStore docStore;
        private string testSuiteRunningFolder;

        internal void LoadDocumentStoreAndInitialize(string clientDllPath, string testSuiteRunningFolder, int databasePort)
        {
            this.testSuiteRunningFolder = testSuiteRunningFolder;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            this.assembly = Assembly.LoadFile(clientDllPath);
            var type = this.assembly.GetType("Raven.Client.Document.DocumentStore");
            docStore = Activator.CreateInstance(type) as Client.Document.DocumentStore;
            docStore.Url = "http://localhost:" + databasePort;
            docStore.DefaultDatabase = "World";
            docStore.Initialize();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender,
                                              ResolveEventArgs args)
        {
            var assemblyname = new AssemblyName(args.Name).Name;
            var assemblyFileName = Path.Combine(this.testSuiteRunningFolder, assemblyname + ".dll");
            var assembly = Assembly.LoadFrom(assemblyFileName);
            return assembly;
        }

        public string GetVersion()
        {
            return "2.5.2750";
        }

        public void Execute(Action<IDocumentStoreWrapper> action)
        {
            action(this);
        }

        public IDocumentSessionWrapper OpenSession()
        {
            return new DocumentSessionWrapper(this.docStore.OpenSession());
        }
    }
}
