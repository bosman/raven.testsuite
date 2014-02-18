using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Client.Wpf.Models;

namespace Raven.TestSuite.Client.Wpf.ViewModels
{
    public class TestViewModel : TreeViewItemViewModel
    {
        readonly TestModel test;

        public TestViewModel(TestModel test, TestGroupViewModel parentGroup)
            : base(parentGroup)
        {
            this.test = test;
        }

        public string TestName
        {
            get { return test.Name; }
        }
    }
}
