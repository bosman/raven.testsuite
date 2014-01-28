using System;
using System.Collections.Generic;
using Raven.TestSuite.Common.Abstractions.Enums;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IIndexDefinitionWrapper
    {
        string Name { get; set; }

        string Map { get; set; }

        HashSet<string> Maps { get; set; }

        string Reduce { get; set; }

        bool IsMapReduce { get; }

        bool IsCompiled { get; set; }

        IDictionary<string, FieldStorageWrapper> Stores { get; set; }

        IDictionary<string, FieldIndexingWrapper> Indexes { get; set; }

        IDictionary<string, SortOptionsWrapper> SortOptions { get; set; }

        IDictionary<string, string> Analyzers { get; set; }

        IList<string> Fields { get; set; }

        IDictionary<string, ISuggestionOptionsWrapper> Suggestions { get; set; }

        IDictionary<string, FieldTermVectorWrapper> TermVectors { get; set; }

        IDictionary<string, ISpatialOptionsWrapper> SpatialIndexes { get; set; }

        IDictionary<string, string> InternalFieldsMapping { get; set; }

        string Type { get; }

    }

}