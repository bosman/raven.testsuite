﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Common
{
    public class TestRun
    {
        public string RavenVersion { get; set; }

        public List<TestResult> TestResults { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime StoppedAt { get; set; }
    }
}