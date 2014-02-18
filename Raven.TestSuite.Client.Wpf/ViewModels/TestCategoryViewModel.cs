using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Client.Wpf.Models;

namespace Raven.TestSuite.Client.Wpf.ViewModels
{
    public class TestCategoryViewModel : TreeViewItemViewModel
    {
        private readonly TestCategoryModel testCategoryModel;

        public TestCategoryViewModel(TestCategoryModel testCategoryModel) 
            : base(null)
        {
            this.testCategoryModel = testCategoryModel;
            foreach (var testGroup in testCategoryModel.TestGroups)
            {
                this.Children.Add(new TestGroupViewModel(testGroup, this));
            }
        }

        public string TestCategoryName
        {
            get { return testCategoryModel.Name; }
        }
    }
}
