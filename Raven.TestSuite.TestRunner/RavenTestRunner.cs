﻿using Raven.TestSuite.Common;
using Raven.TestSuite.Common.Abstractions;
using Raven.TestSuite.Common.WrapperInterfaces;
using Raven.TestSuite.Tests.Common;
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
    public class RavenTestRunner : IDisposable
    {
        private DbRunner dbRunner;

        public Task<List<TestRun>> RunAllTests(IProgress<ProgressReport> progress, CancellationToken token, TestRunSetup testRunSetup)
        {
            var task = Task.Factory.StartNew<List<TestRun>>(() =>
                {
                    var allRuns = new List<TestRun>();

                    foreach (var ravenVersionFolderPath in testRunSetup.RavenVersionPath)
                    {
                        try
                        {
                            if (!Directory.Exists(ravenVersionFolderPath))
                            {
                                continue;
                            }
                            this.Cleanup();

                            var testGroups = GetAllRavenDotNetApiTests();
                            var restGroups = GetAllRavenRestApiTests();

                            var testRun = new TestRun
                                {
                                    RavenVersion = VersionPicker.GetRavenVersionByFolder(ravenVersionFolderPath),
                                    StartedAt = DateTime.Now
                                };

                            var smugglerGroups = GetAllRavenTestsByType(typeof (RavenSmugglerTestAttribute));
                            var testResults = new List<TestResult>();

                            var dbPort = 8080;

                            var serverStandaloneExePath = Path.Combine(ravenVersionFolderPath,
                                                                       Constants.Paths.ServerStandaloneExePartialPath);

                            var domainContainer =
                                VersionPicker.TryCreateDomainContainerForRavenVersion(ravenVersionFolderPath, dbPort);
                            if (domainContainer != null)
                            {
                                using (dbRunner = DbRunner.Run(dbPort, serverStandaloneExePath))
                                {
                                    Console.WriteLine("Server startup time: {0}s", dbRunner.StartupTime.TotalSeconds);

                                    using (domainContainer)
                                    {
                                        var wrapper = domainContainer.Wrapper;
                                        System.Console.WriteLine("Using .Net wrapper version: " + wrapper.GetVersion());

                                        foreach (var testGroup in testGroups)
                                        {
                                            InterruptExecutionIfCancellationRequested(token);
                                            var obj = Activator.CreateInstance(testGroup.GroupType,
                                                                               new object[] {wrapper});
                                            foreach (var test in testGroup.Tests)
                                            {
                                                InterruptExecutionIfCancellationRequested(token);
                                                RunExecutableAttributesCodeBeforeTest(test, wrapper);
                                                var result = RunTest(testGroup, test, obj);
                                                testResults.Add(result);
                                                ReportResultAsProgressReport(progress, result);
                                            }
                                        }

                                        foreach (var restGroup in restGroups)
                                        {
                                            InterruptExecutionIfCancellationRequested(token);
                                            var obj = Activator.CreateInstance(restGroup.GroupType,
                                                                               new object[] {wrapper});
                                            foreach (var test in restGroup.Tests)
                                            {
                                                InterruptExecutionIfCancellationRequested(token);
                                                RunExecutableAttributesCodeBeforeTest(test, wrapper);
                                                var result = RunTest(restGroup, test, obj);
                                                testResults.Add(result);
                                                ReportResultAsProgressReport(progress, result);
                                            }
                                        }

                                        foreach (var smugglerGroup in smugglerGroups)
                                        {
                                            InterruptExecutionIfCancellationRequested(token);
                                            var obj = Activator.CreateInstance(smugglerGroup.GroupType,
                                                                               new object[] {wrapper});
                                            foreach (var test in smugglerGroup.Tests)
                                            {
                                                InterruptExecutionIfCancellationRequested(token);
                                                RunExecutableAttributesCodeBeforeTest(test, wrapper);
                                                var result = RunTest(smugglerGroup, test, obj);
                                                testResults.Add(result);
                                                ReportResultAsProgressReport(progress, result);
                                            }
                                        }
                                    }
                                }
                            }
                            testRun.StoppedAt = DateTime.Now;
                            testRun.TestResults = testResults;
                            allRuns.Add(testRun);
                        }
                        catch (Exception ex)
                        {
                            // TODO add notification to send to UI that the whole thing failed
                        }
                    }

                    return allRuns;

                }, token);
            return task;
        }

        private void Cleanup()
        {
            if (this.dbRunner == null)
                return;

            this.dbRunner.Dispose();
            this.dbRunner = null;
        }

        private static void InterruptExecutionIfCancellationRequested(CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                throw new OperationCanceledException(token);
            }
        }

        private static void ReportResultAsProgressReport(IProgress<ProgressReport> progress, TestResult result)
        {
            progress.Report(new ProgressReport
                {
                    Message = result.TestName + " : " + (result.IsSuccess ? "Passed" : "Failed")
                });
        }

        private static void RunExecutableAttributesCodeBeforeTest(MemberInfo test, IRavenClientWrapper wrapper)
        {
            var executableAttribs = test.GetCustomAttributes<ExecutableAttribute>();
            foreach (var executableAttrib in executableAttribs)
            {
                wrapper.Execute(executableAttrib.GetExecutableAction());
            }
        }

        private static TestResult RunTest(RavenTestsGroup testGroup, MethodInfo test, object obj)
        {
            var watch = new Stopwatch();
            var ravenTestAttribute = test.GetCustomAttribute<RavenTestAttribute>();
            var result = new TestResult { TestName = testGroup.GroupType.FullName + "." + test.Name, TestType = ravenTestAttribute.TestTypeName };
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
            return result;
        }

        public IEnumerable<RavenTestsGroup> GetAllRavenDotNetApiTests()
        {
            return GetAllRavenTestsByType(typeof(RavenDotNetApiTestAttribute));
        }

        public IEnumerable<RavenTestsGroup> GetAllRavenRestApiTests()
        {
            return GetAllRavenTestsByType(typeof(RavenRestApiTestAttribute));
        }

        public IEnumerable<RavenTestsGroup> GetAllRavenTestsByType(Type revenTestAttributeType)
        {
            var ass = AppDomain.CurrentDomain.Load("Raven.TestSuite.Tests");

            var ravTestGr =
                ass.GetTypes()
                   .Where(
                       t =>
                       t.GetMethods()
                        .Any(m => m.GetCustomAttributes(revenTestAttributeType, false).Length > 0))
                   .Select(groupType => new RavenTestsGroup
                   {
                       GroupType = groupType,
                       Tests = groupType.GetMethods()
                                .Where(
                                    m => m.GetCustomAttributes(revenTestAttributeType, false).Length > 0).ToList()
                   }).ToList();

            return ravTestGr;
        }

        public void Dispose()
        {
            this.Cleanup();
        }
    }
}
