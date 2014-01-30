﻿using System.IO;
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

                    env.RunSmuggler("out http://localhost:" + env.DbPort + " " + pathToDump);

                    Assert.True(File.Exists(pathToDump));
                });
        }
    }
}
