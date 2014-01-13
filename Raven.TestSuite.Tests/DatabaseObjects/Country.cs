using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raven.TestSuite.Tests.DatabaseObjects
{
    public class Country
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public long Area { get; set; }
        public string Capital { get; set; }
        public string Province { get; set; }
    }
}
