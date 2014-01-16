using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Raven.TestSuite.Common;
using Raven.TestSuite.Common.Attributes;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Raven.TestSuite.TestRunner
{
    public class RavenTestRunner
    {
        public Task<List<TestResult>> RunAllTests(IProgress<ProgressReport> progress, CancellationToken token)
        {
            var task = Task.Factory.StartNew<List<TestResult>>(() =>
                {
                    var testMethods = GetAllRavenTests();

                    var testResults = new List<TestResult>();
                    Stopwatch watch = new Stopwatch();

                    var dbPort = 8080;

                    using (var dbRunner = DbRunner.Run(dbPort))
                    {
                        using (var domainContainer = new ClientWrapper.v2_5_2750.DomainContainer(
                            "C:\\RavenDB-Build-2750\\Client\\Raven.Client.Lightweight.dll", "version1", dbPort))
                        {
                            var wrapper = domainContainer.Wrapper;
                            System.Console.WriteLine(wrapper.GetVersion());

                            foreach (var testMethod in testMethods)
                            {
                                if (token.IsCancellationRequested)
                                {
                                    throw new OperationCanceledException(token);
                                }
                                watch.Reset();
                                var result = new TestResult();
                                result.TestName = testMethod.Item2.Name;
                                var obj = Activator.CreateInstance(testMethod.Item1, new object[] {wrapper});
                                try
                                {
                                    watch.Start();
                                    testMethod.Item2.Invoke(obj, new object[0]);
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

                    return testResults;
                }, token);
            return task;
        }

        public IEnumerable<Tuple<Type, MethodInfo>>  GetAllRavenTests()
        {
            //making sure the assembly is loaded
            var testsAssembly = Assembly.Load("Raven.TestSuite.Tests");
            var types =
                AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(
                             assembly =>
                             assembly.GetTypes().Where(t => t.IsSubclassOf(typeof (TestGroupBase))).ToList());
            var methodsAndTypes = types.SelectMany(
                t => t.GetMethods()
                      .Where(m => m.GetCustomAttributes(typeof (RavenTestAttribute), false).Length > 0)
                      .Select(tm => new Tuple<Type, MethodInfo> (t, tm))
                );

            return methodsAndTypes;
        }
    }
}
