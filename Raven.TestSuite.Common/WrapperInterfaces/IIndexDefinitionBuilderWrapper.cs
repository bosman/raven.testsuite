﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Raven.TestSuite.Common.Abstractions.Enums;
using Raven.TestSuite.Common.Indexing;
using Raven.TestSuite.Common.Data;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IIndexDefinitionBuilderWrapper<TDocument, TReduceResult>
    {
		Expression<Func<IEnumerable<TDocument>, IEnumerable>> Map { get; set; }
		Expression<Func<IEnumerable<TReduceResult>, IEnumerable>> Reduce { get; set; }

		IDictionary<Expression<Func<TReduceResult, object>>, FieldStorageWrapper> Stores { get; set; }

		IDictionary<string, FieldStorageWrapper> StoresStrings { get; set; }

		IDictionary<Expression<Func<TReduceResult, object>>, FieldIndexingWrapper> Indexes { get; set; }

        IDictionary<string, FieldIndexingWrapper> IndexesStrings { get; set; }

		IDictionary<Expression<Func<TReduceResult, object>>, SortOptionsWrapper> SortOptions { get; set; }

        Dictionary<string, SortOptionsWrapper> SortOptionsStrings { get; set; }

		IDictionary<Expression<Func<TReduceResult, object>>, string> Analyzers { get; set; }

		IDictionary<string, string> AnalyzersStrings { get; set; }

        IDictionary<Expression<Func<TReduceResult, object>>, SuggestionOptionsWrapper> Suggestions { get; set; }

		IDictionary<Expression<Func<TReduceResult, object>>, FieldTermVectorWrapper> TermVectors { get; set; }

        IDictionary<string, FieldTermVectorWrapper> TermVectorsStrings { get; set; }

		IDictionary<Expression<Func<TReduceResult, object>>, SpatialOptionsWrapper> SpatialIndexes { get; set; }

        IDictionary<string, SpatialOptionsWrapper> SpatialIndexesStrings { get; set; }

	    IndexDefinitionWrapper ToIndexDefinition(IDocumentConventionWrapper convention, bool validateMap = true);



    }
}