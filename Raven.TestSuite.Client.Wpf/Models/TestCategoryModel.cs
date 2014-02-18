using System.Collections.Generic;

namespace Raven.TestSuite.Client.Wpf.Models
{
    public class TestCategoryModel
    {
        private List<TestGroupModel> testGroups = new List<TestGroupModel>();

        public string Name { get; set; }

        public List<TestGroupModel> TestGroups
        {
            get { return testGroups; }
            set { testGroups = value; }
        }
    }
}
