using Raven.TestSuite.ClientWrapper._2_5_2750;
using Raven.TestSuite.Common.WrapperInterfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public partial class Wrapper : MarshalByRefObject, IRavenClientWrapper, ITestUnitEnvironment, IRestEnvironment
    {
        private Assembly assembly;
        private int databasePort;
        private string testSuiteRunningFolder;

        public Wrapper()
        {
        }

        private Wrapper(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public static Wrapper Create()
        {
            return new Wrapper(Assembly.Load("Raven.Client.Lightweight"));
        }

        internal void LoadAssemblyAndSetUp(string clientDllPath, string testSuiteRunningFolder, int databasePort)
        {
            this.testSuiteRunningFolder = testSuiteRunningFolder;
            this.databasePort = databasePort;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            this.assembly = Assembly.LoadFile(clientDllPath);
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

        public void Execute(Action<IRavenClientWrapper> action)
        {
            action(this);
        }

#region ITestUnitEnvironment

        public void Execute(Action<ITestUnitEnvironment> action)
        {
            action(this);
        }

        public IDocumentStoreWrapper CreateDocumentStore(string defaultDatabase)
        {
            var type = this.assembly.GetType("Raven.Client.Document.DocumentStore");
            var documentStore = Activator.CreateInstance(type) as Client.Document.DocumentStore;
            documentStore.Url = "http://localhost:" + this.databasePort;
            documentStore.DefaultDatabase = defaultDatabase;
            return new DocumentStoreWrapper(documentStore);
        }

#endregion
        
#region IRestEnvironment

        public void Execute(Action<IRestEnvironment> action)
        {
            action(this);
        }

        public HttpResponseMessage RawGet(string url)
        {
            var client = new HttpClient();
            var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
            task.Wait();
            return task.Result;
        }

        public HttpResponseMessage RawPut(string url, string content)
        {
            var client = new HttpClient();
            var task = client.PutAsync(url, new StringContent(content));
            task.Wait();
            return task.Result;
        }

        public HttpResponseMessage RawPost(string url, string content)
        {
            var client = new HttpClient();
            var task = client.PostAsync(url, new StringContent(content));
            task.Wait();
            return task.Result;
        }

        public HttpResponseMessage RawDelete(string url)
        {
            var client = new HttpClient();
            var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, url));
            task.Wait();
            return task.Result;
        }

#endregion
    }
}
