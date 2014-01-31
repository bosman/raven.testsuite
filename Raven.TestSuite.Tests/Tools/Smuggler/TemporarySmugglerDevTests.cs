using System;
using System.IO;
using Raven.TestSuite.Common.WrapperInterfaces;
using Raven.TestSuite.Tests.Common;
using Raven.TestSuite.Tests.Common.Attributes;
using Xunit;

namespace Raven.TestSuite.Tests.Tools.Smuggler
{
    public class TemporarySmugglerDevTests : BaseTestGroup
    {
        public TemporarySmugglerDevTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenSmugglerTest]
        public void SomeTest()
        {
            this.wrapper.Execute(env =>
                {
                    var pathToDump = @"C:\temp\dump.raven";
                    if (File.Exists(pathToDump))
                    {
                        File.Delete(pathToDump);
                    }
                    var args = env.SmugglerArgsBuilder().ExportFrom("http://localhost:" + env.DbPort).UsingFile(pathToDump).Build();
                    env.RunSmuggler(args);

                    Assert.True(File.Exists(pathToDump));
                });
        }

        [RavenSmugglerTest]
        public void SomeOtherTest()
        {
            this.wrapper.Execute(env =>
            {
                var pathToDump = @"C:\temp\dump2.raven";
                if (File.Exists(pathToDump))
                {
                    File.Delete(pathToDump);
                }

                var args = env.SmugglerArgsBuilder().ExportFrom(env.DefaultDbAddress).UsingFile(pathToDump).Build();
                env.RunSmuggler(args);

                Assert.True(File.Exists(pathToDump));
            });
        }

        [RavenSmugglerTest]
        public void YetAnotherTest()
        {
            this.wrapper.Execute(env =>
            {
                var pathToDump = @"C:\temp\dump3.raven";
                if (File.Exists(pathToDump))
                {
                    File.Delete(pathToDump);
                }

                var args = env.SmugglerArgsBuilder().ExportFrom(env.DefaultDbAddress).UsingFile(pathToDump).UsingDatabase("World").WithExcludeExpired().Build();
                env.RunSmuggler(args);

                Assert.True(File.Exists(pathToDump));
            });
        }

        [RavenSmugglerTest]
        public void FailingTest()
        {
            throw new Exception("Test");
        }
    }
}
