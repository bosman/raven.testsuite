using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Raven.TestSuite.Common;
using Raven.TestSuite.Common.DatabaseObjects;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class Wrapper : MarshalByRefObject, IRavenClientWrapper
    {
        private Assembly assembly;
        private Client.Document.DocumentStore docStore;

        internal void LoadDocumentStoreAndInitialize(string path)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            this.assembly = Assembly.LoadFile(path);
            var type = this.assembly.GetType("Raven.Client.Document.DocumentStore");
            docStore = Activator.CreateInstance(type) as Client.Document.DocumentStore;
            docStore.Url = "http://localhost:8080";
            docStore.DefaultDatabase = "World";
            docStore.Initialize();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender,
                                              ResolveEventArgs args)
        {
            var assemblyname = new AssemblyName(args.Name).Name;
            var assemblyFileName = Path.Combine("C:\\Workspaces\\raven.testsuite\\Raven.TestSuite.Client.Console\\bin\\Debug", assemblyname + ".dll");
            var assembly = Assembly.LoadFrom(assemblyFileName);
            return assembly;
        }

        public string GetVersion()
        {
            return "2.5.2750";
        }

        public IList<string> GetSomeStrings()
        {
            using (var session = docStore.OpenSession())
            {
                var countries = session.Query<Country>().Where(o => o.Area > 1000000);
                return countries.Select(x => x.Capital).ToList();
            }
        }

        public T DoInSession<T>(Func<IDocumentSessionWrapper, T> expression)
        {
            using (var session = new DocumentSessionWrapper(docStore.OpenSession()))
            {
                return expression(session);
            }
        }

        public K QueryInSession<T, K>(Func<IQueryable<T>, K> expression)
        {
            using (var session = docStore.OpenSession())
            {
                return expression(session.Query<T>());
            }
        }
    }
}
