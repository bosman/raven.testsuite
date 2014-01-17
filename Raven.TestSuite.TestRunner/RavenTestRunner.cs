using Raven.TestSuite.Common;
using Raven.TestSuite.Tests.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Raven.TestSuite.TestRunner
{
    public class RavenTestRunner
    {
        public Task<List<TestResult>> RunAllTests(IProgress<ProgressReport> progress, CancellationToken token, string ravenVersionFolderPath)
        {
            var task = Task.Factory.StartNew<List<TestResult>>(() =>
                {
                    var testGroups = GetAllRavenDotNetApiTests();

                    var testResults = new List<TestResult>();
                    var watch = new Stopwatch();

                    var dbPort = 8080;

                    var clientDllPath = Path.Combine(ravenVersionFolderPath, Constants.Paths.ClientDllPartialPath);
                    var serverStandaloneExePath = Path.Combine(ravenVersionFolderPath, Constants.Paths.ServerStandaloneExePartialPath);

                    using (var dbRunner = DbRunner.Run(dbPort, serverStandaloneExePath))
                    {
                        using (var domainContainer = new ClientWrapper.v2_5_2750.DomainContainer(
                            clientDllPath, "version1", dbPort))
                        {
                            var wrapper = domainContainer.Wrapper;
                            System.Console.WriteLine("Using .Net wrapper version: " + wrapper.GetVersion());


                            foreach (var testGroup in testGroups)
                            {
                                if (token.IsCancellationRequested)
                                {
                                    throw new OperationCanceledException(token);
                                }
                                var obj = Activator.CreateInstance(testGroup.GroupType, new object[] { wrapper });
                                foreach (var test in testGroup.Tests)
                                {
                                    if (token.IsCancellationRequested)
                                    {
                                        throw new OperationCanceledException(token);
                                    }
                                    watch.Reset();
                                    var result = new TestResult();
                                    result.TestName = testGroup.GroupType.FullName + "." + test.Name;
                                    try
                                    {
                                        watch.Start();
                                        test.Invoke(obj, new object[0]);
                                        result.IsSuccess = true;
                                    }
                                    catch (TargetInvocationException ex)
                                    {
                                        result.IsSuccess = false;
                                        result.Exception = ex.InnerException;
                                    }
                                    catch (Exception ex)
                                    {
                                        result.IsSuccess = false;
                                        result.Exception = ex;
                                    }
                                    finally
                                    {
                                        watch.Stop();
                                        result.ExecutionTime = watch.Elapsed;
                                    }
                                    testResults.Add(result);
                                    progress.Report(new ProgressReport
                                    {
                                        Message = result.TestName + " : " + (result.IsSuccess ? "Passed" : "Failed")
                                    });
                                }
                            }
                        }
                    }

                    return testResults;
                }, token);
            return task;
        }

        public IEnumerable<RavenTestsGroup> GetAllRavenDotNetApiTests()
        {
            var ass = AppDomain.CurrentDomain.Load("Raven.TestSuite.Tests");

            var ravTestGr =
                ass.GetTypes()
                   .Where(
                       t =>
                       t.GetMethods()
                        .Any(m => m.GetCustomAttributes(typeof (RavenDotNetApiTestAttribute), false).Length > 0))
                   .Select(groupType => new RavenTestsGroup
                       {
                           GroupType = groupType,
                           Tests = groupType.GetMethods()
                                    .Where(
                                        m => m.GetCustomAttributes(typeof(RavenDotNetApiTestAttribute), false).Length > 0).ToList()
                       }).ToList();

            return ravTestGr;
        }
    }
}
