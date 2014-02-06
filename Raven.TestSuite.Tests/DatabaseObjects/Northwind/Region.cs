using System.Collections.Generic;

namespace Raven.TestSuite.Tests.DatabaseObjects.Northwind
{
    public class Region
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Territory> Territories { get; set; }
    }
}
