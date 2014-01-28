using System;
using System.Collections.Generic;
using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Enums;
using Raven.TestSuite.Common.Abstractions.Json.Linq;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IIndexQueryWrapper
    {
        bool PageSizeSet { get; }
        string Query { get; set; }
        Dictionary<string, RavenJTokenWrapper> QueryInputs { get; set; }
        int Start { get; set; }
        int PageSize { get; }
        string[] GroupBy { get; set; }
        string[] FieldsToFetch { get; set; }
        ISortedFieldWrapper[] SortedFields { get; set; }
        DateTime? Cutoff { get; set; }
        EtagWrapper CutoffEtag { get; set; }
        string DefaultField { get; set; }
        QueryOperatorWrapper DefaultOperator { get; set; }
        bool SkipTransformResults { get; set; }
        IReferenceWrapper<int> SkippedResults { get; set; }
        IHighlightedFieldWrapper[] HighlightedFields { get; set; }
        string[] HighlighterPreTags { get; set; }
        string[] HighlighterPostTags { get; set; }
        string ResultsTransformer { get; set; }
        bool DisableCaching { get; set; }

        string GetIndexQueryUrl(string operationUrl, string index, string operationName,
                                bool includePageSizeEvenIfNotExplicitlySet = true);
    }
}