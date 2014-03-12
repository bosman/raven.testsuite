using System.Diagnostics;
using Raven.TestSuite.ClientWrapper.v2_5_2750.CommandLineTools;
using Raven.TestSuite.Common;
using Raven.TestSuite.Common.Abstractions.Json.Linq;
using Raven.TestSuite.Common.WrapperInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public partial class Wrapper : MarshalByRefObject, IRavenClientWrapper, ITestUnitEnvironment
    {
        private Assembly assembly;
        private int databasePort;
        private string testSuiteRunningFolder;
        private ToolsRunner toolsRunner;
        private string ravenDllVersion;

        public Wrapper()
        {
        }

        private Wrapper(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public int DbPort
        {
            get { return this.databasePort; }
        }

        public string DefaultDbAddress
        {
            get { return "http://localhost:" + this.databasePort; }
        }

        public static Wrapper Create()
        {
            return new Wrapper(Assembly.Load("Raven.Client.Lightweight"));
        }

        internal void LoadAssemblyAndSetUp(string clientDllPath, string ravenVersionFolderPath, string testSuiteRunningFolder, int databasePort)
        {
            this.testSuiteRunningFolder = testSuiteRunningFolder;
            this.databasePort = databasePort;
            this.toolsRunner = new ToolsRunner(ravenVersionFolderPath);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(clientDllPath);
            ravenDllVersion = fileVersionInfo.FileVersion;
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

        public string GetRavenDllVersion()
        {
            return ravenDllVersion;
        }

        public string GetWrapperVersion()
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

        #region REST HTTP Api

        public RestResponse RawGet(string url, string query = null)
        {
            var client = new HttpClient();
            var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, CompleteUrlIfNeeded(url, query)));
            return new RestResponse { RawResponse = task.Result, RavenJTokenWrapper = HttpResponseMessageToRavenJTokenWrapper(task.Result) };
        }

        public RestResponse RawPatch(string url, string content, string query = null, Dictionary<string, List<string>> headers = null)
        {
            var request = new HttpRequestMessage();
            if (headers != null)
            {
                foreach (KeyValuePair<string, List<string>> header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            request.Content = new StringContent(content);
            request.RequestUri = new Uri(CompleteUrlIfNeeded(url, query));
            request.Method = new HttpMethod("PATCH");

            var client = new HttpClient();
            var task = client.SendAsync(request);
            return new RestResponse { RawResponse = task.Result, RavenJTokenWrapper = HttpResponseMessageToRavenJTokenWrapper(task.Result) };
        }

        public RestResponse RawPut(string url, string content, Dictionary<string, List<string>> headers = null)
        {
            var request = new HttpRequestMessage();
            if (headers != null)
            {
                foreach (KeyValuePair<string, List<string>> header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            request.Content = new StringContent(content);
            request.RequestUri = new Uri(CompleteUrlIfNeeded(url));
            request.Method = HttpMethod.Put;

            var client = new HttpClient();
            var task = client.SendAsync(request);
            return new RestResponse { RawResponse = task.Result, RavenJTokenWrapper = HttpResponseMessageToRavenJTokenWrapper(task.Result) };
        }

        public RestResponse RawPost(string url, string content, string query = null)
        {
            var client = new HttpClient();
            var task = client.PostAsync(CompleteUrlIfNeeded(url, query), new StringContent(content));
            return new RestResponse { RawResponse = task.Result, RavenJTokenWrapper = HttpResponseMessageToRavenJTokenWrapper(task.Result) };
        }

        public RestResponse RawDelete(string url, string query = null)
        {
            var client = new HttpClient();
            var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, CompleteUrlIfNeeded(url, query)));
            return new RestResponse { RawResponse = task.Result, RavenJTokenWrapper = null };
        }

        private string CompleteUrlIfNeeded(string url, string query = null)
        {
            Uri result;
            if (Uri.TryCreate(url, UriKind.Absolute, out result))
            {
                return result.ToString();
            }
            var ub = new UriBuilder("http", "localhost", databasePort, url);
            ub.Query = query;
            return ub.ToString();
        }

        private RavenJTokenWrapper HttpResponseMessageToRavenJTokenWrapper(HttpResponseMessage httpResponseMessage)
        {
            var resultContent = httpResponseMessage.Content.ReadAsStringAsync();
            try
            {
                return RavenJTokenWrapper.Parse(resultContent.Result);
            }
            catch 
            {
                return null;
            }
        }

        #endregion

        #region Tools

        public void RunSmuggler(string arguments)
        {
            this.toolsRunner.RunSmuggler(arguments);
        }

        public ISmugglerArgumentsBuilder SmugglerArgsBuilder()
        {
            return new SmugglerArgumentsBuilder();
        }

        #endregion
    }
}
