using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven.TestSuite.Client.Wpf.Models
{
    public class TestGroupModel
    {
        private List<TestModel> tests = new List<TestModel>();

        public string Name { get; set; }

        public List<TestModel> Tests
        {
            get { return tests; }
            set { tests = value; }
        }
    }
}
