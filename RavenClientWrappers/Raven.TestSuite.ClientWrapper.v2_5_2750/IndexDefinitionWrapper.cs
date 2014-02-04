using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common.Abstractions.Enums;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class IndexDefinitionWrapper : IIndexDefinitionWrapper
    {
        public IndexDefinitionWrapper()
        {
            Maps = new HashSet<string>();
            Indexes = new Dictionary<string, FieldIndexingWrapper>();
            Stores = new Dictionary<string, FieldStorageWrapper>();
            Analyzers = new Dictionary<string, string>();
            SortOptions = new Dictionary<string, SortOptionsWrapper>();
            Fields = new List<string>();
            Suggestions = new Dictionary<string, ISuggestionOptionsWrapper>();
            TermVectors = new Dictionary<string, FieldTermVectorWrapper>();
            SpatialIndexes = new Dictionary<string, ISpatialOptionsWrapper>();
        }

        public string Name { get; set; }

        public string Map
        {
            get
            {
                return Maps.FirstOrDefault();
            }
            set
            {
                if (Maps.Count != 0)
                {
                    Maps.Remove(Maps.First());
                }
                Maps.Add(value);
            }
        }

        public HashSet<string> Maps { get; set; }

        public string Reduce { get; set; }

        public bool IsMapReduce
        {
            get { return !string.IsNullOrEmpty(Reduce); }
        }

        public bool IsCompiled { get; set; }

        public IDictionary<string, FieldStorageWrapper> Stores { get; set; }

        public IDictionary<string, FieldIndexingWrapper> Indexes { get; set; }

        public IDictionary<string, SortOptionsWrapper> SortOptions { get; set; }

        public IDictionary<string, string> Analyzers { get; set; }

        public IList<string> Fields { get; set; }

        public IDictionary<string, ISuggestionOptionsWrapper> Suggestions { get; set; }

        public IDictionary<string, FieldTermVectorWrapper> TermVectors { get; set; }

        public IDictionary<string, ISpatialOptionsWrapper> SpatialIndexes { get; set; }

        public IDictionary<string, string> InternalFieldsMapping { get; set; }

        public string Type
        {
            get
            {
                var name = Name ?? string.Empty;
                if (name.StartsWith("Auto/", StringComparison.OrdinalIgnoreCase))
                    return "Auto";
                if (IsCompiled)
                    return "Compiled";
                if (IsMapReduce)
                    return "MapReduce";
                return "Map";
            }
        }
    }
}
