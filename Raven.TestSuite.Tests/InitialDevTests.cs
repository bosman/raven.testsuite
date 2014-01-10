using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Raven.TestSuite.Common;
using Raven.TestSuite.Common.Attributes;
using Raven.TestSuite.Common.DatabaseObjects;

namespace Raven.TestSuite.Tests
{
    public class InitialDevTests : TestGroupBase
    {
        private IRavenClientWrapper wrapper;

        public InitialDevTests(IRavenClientWrapper wrapper)
        {
            this.wrapper = wrapper;
        }

        [RavenTest]
        public void SomeTest()
        {
            var results = wrapper.DoInSession<List<string>>(
                x =>
                {
                    var capsOverMillion = x.Query<Country>().Where(o => o.Area > 1000000).Count();
                    var capsOverThousand = x.Query<Country>().Where(o => o.Area > 1000).Count();
                    return new List<string>() { capsOverMillion.ToString(), capsOverThousand.ToString() };
                }
                );
            foreach (var result in results)
            {
                System.Console.WriteLine(result);
            }
        }

        [RavenTest]
        public void TestThatThrowsException()
        {
            var results = wrapper.DoInSession<List<string>>(
                x =>
                {
                    var capsOverMillion = x.Query<Country>().Where(o => o.Area > 1000000).Count();
                    throw new Exception("This exception is thrown on purpose");
                    var capsOverThousand = x.Query<Country>().Where(o => o.Area > 1000).Count();
                    return new List<string>() { capsOverMillion.ToString(), capsOverThousand.ToString() };
                }
                );
        }
    }
}
