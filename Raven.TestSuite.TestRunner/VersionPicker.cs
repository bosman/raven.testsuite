using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common.Abstractions;
using System.IO;
using System.Diagnostics;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.TestRunner
{
    public static class VersionPicker
    {
        private static List<Tuple<string, Type>> versionsMap = new List<Tuple<string, Type>>
            {
                //Order these items starting from the highest version
                new Tuple<string, Type>("2.5.2755.0", typeof(ClientWrapper.v2_5_2750.DomainContainer)),
                new Tuple<string, Type>("2.5.2750.0", typeof(ClientWrapper.v2_5_2750.DomainContainer))
            };

        public static IDomainContainer TryCreateDomainContainerForRavenVersion(string ravenVersionFolderPath, int databasePort)
        {
            var ravenClientDllPath = Path.Combine(ravenVersionFolderPath, Constants.Paths.ClientDllPartialPath);
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(ravenClientDllPath);

            foreach (var version in versionsMap)
            {
                if (fileVersionInfo.FileVersion.CompareTo(version.Item1) >= 0)
                {
                    return (IDomainContainer)Activator.CreateInstance(version.Item2, new object[] { ravenVersionFolderPath, version.Item1, databasePort });
                }
            }
            return null;
        }

        public static string GetRavenVersionByFolder(string ravenVersionFolderPath)
        {
            var ravenClientDllPath = Path.Combine(ravenVersionFolderPath, Constants.Paths.ClientDllPartialPath);
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(ravenClientDllPath);
            return fileVersionInfo.FileVersion;
        }
    }
}
