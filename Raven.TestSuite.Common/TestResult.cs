﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Imports.Newtonsoft.Json;

namespace Raven.TestSuite.Common
{
    public class TestResult
    {
        public int Id { get; set; }

        public string TestName { get; set; }

        public string HumanTestName
        {
            get { return TestName.Replace("Raven.TestSuite.Tests.", string.Empty); }
        }

        public string Color
        {
            get { return IsSuccess ? "PaleGreen" : "Tomato"; }
        }

        public string TestType { get; set; }

        public bool IsSuccess { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public Exception Exception { get; set; }
    }
}
