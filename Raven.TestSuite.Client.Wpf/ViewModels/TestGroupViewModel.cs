using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Client.Wpf.Models;

namespace Raven.TestSuite.Client.Wpf.ViewModels
{
    public class TestGroupViewModel : TreeViewItemViewModel
    {
        private readonly TestGroupModel testGroupModel;

        public TestGroupViewModel(TestGroupModel testGroupModel, TestCategoryViewModel parentCategory)
            : base(parentCategory)
        {
            this.testGroupModel = testGroupModel;
            foreach (var test in testGroupModel.Tests)
            {
                this.Children.Add(new TestViewModel(test, this));
            }
        }

        public string TestGroupName
        {
            get { return testGroupModel.Name; }
        }
    }
}
