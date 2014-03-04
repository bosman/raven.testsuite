using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Json.Linq;
using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Enums;
using Raven.TestSuite.Common.Abstractions.Json.Linq;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750.Extensions
{
    public static class WrapperExtensions
    {
        public static RavenJToken Unwrap(this RavenJTokenWrapper wrapped)
        {
            return RavenJToken.Parse(wrapped.ToString());
        }

        public static RavenJTokenWrapper Wrap(this RavenJToken unwrapped)
        {
            return unwrapped != null ? RavenJTokenWrapper.Parse(unwrapped.ToString()) : null;
        }

        public static Etag Unwrap(this EtagWrapper wrapped)
        {
            return new Etag(wrapped.ToString());
        }

        public static EtagWrapper Wrap(this Etag etag)
        {
            return etag != null ? EtagWrapper.Parse(etag.ToString()) : null;
        }

        public static RavenJObject Unwrap(this RavenJObjectWrapper wrapped)
        {
            return RavenJObject.Parse(wrapped.ToString());
        }

        public static RavenJObjectWrapper Wrap(this RavenJObject ravenJObj)
        {
            return ravenJObj != null ? RavenJObjectWrapper.Parse(ravenJObj.ToString()) : null;
        }

        public static IIndexingPerformanceStatsWrapper Wrap(this IndexingPerformanceStats ips)
        {
            if (ips != null)
            {
                var result = new IndexingPerformanceStatsWrapper
                {
                    Operation = ips.Operation,
                    OutputCount = ips.OutputCount,
                    InputCount = ips.InputCount,
                    ItemsCount = ips.ItemsCount,
                    Duration = ips.Duration,
                    Started = ips.Started
                };
                return result;
            }
            return null;
        }

        public static IDatabaseStatisticsWrapper Wrap(this DatabaseStatistics databaseStatistics)
        {
            if (databaseStatistics != null)
            {
                var attachemntWrapper = new DatabaseStatisticsWrapper
                {
                    LastDocEtag = databaseStatistics.LastDocEtag.Wrap(),
                    LastAttachmentEtag = databaseStatistics.LastAttachmentEtag.Wrap(),
                    CountOfIndexes = databaseStatistics.CountOfIndexes,
                    InMemoryIndexingQueueSize = databaseStatistics.InMemoryIndexingQueueSize,
                    ApproximateTaskCount = databaseStatistics.ApproximateTaskCount,
                    CountOfDocuments = databaseStatistics.CountOfDocuments,
                    StaleIndexes = databaseStatistics.StaleIndexes,
                    CurrentNumberOfItemsToIndexInSingleBatch = databaseStatistics.CurrentNumberOfItemsToIndexInSingleBatch,
                    CurrentNumberOfItemsToReduceInSingleBatch = databaseStatistics.CurrentNumberOfItemsToReduceInSingleBatch,
                    DatabaseTransactionVersionSizeInMB = databaseStatistics.DatabaseTransactionVersionSizeInMB,
                    Indexes = databaseStatistics.Indexes.Select(indexStat => indexStat.Wrap()).ToArray(),
                    Errors = databaseStatistics.Errors.Select(err => ServerErrorWrapper.FromServerError(err) as IServerErrorWrapper).ToArray(),
                    Triggers = databaseStatistics.Triggers.Select(t => TriggerInfoWrapper.FromTriggerInfo(t) as ITriggerInfoWrapper).ToArray(),
                    Extensions = databaseStatistics.Extensions.Select(e => e.Wrap()).ToArray(),
                    ActualIndexingBatchSize = databaseStatistics.ActualIndexingBatchSize.Select(a => a.Wrap()).ToArray(),
                    Prefetches = databaseStatistics.Prefetches.Select(p => p.Wrap()).ToArray(),
                    DatabaseId = databaseStatistics.DatabaseId
                };
                return attachemntWrapper;
            }
            return null;
        }

        public static IActualIndexingBatchSizeWrapper Wrap(this ActualIndexingBatchSize aibs)
        {
            if (aibs != null)
            {
                var result = new ActualIndexingBatchSizeWrapper
                {
                    Size = aibs.Size,
                    Timestamp = aibs.Timestamp
                };
                return result;
            }
            return null;
        }

        public static IFutureBatchStatsWrapper Wrap(this FutureBatchStats fbs)
        {
            if (fbs != null)
            {
                var result = new FutureBatchStatsWrapper
                {
                    Timestamp = fbs.Timestamp,
                    Duration = fbs.Duration,
                    Size = fbs.Size,
                    Retries = fbs.Retries
                };
                return result;
            }
            return null;
        }

        public static IExtensionsLogWrapper Wrap(this ExtensionsLog el)
        {
            if (el != null)
            {
                var result = new ExtensionsLogWrapper
                    {
                        Name = el.Name,
                        Installed = el.Installed.Select(i => i.Warp()).ToArray()
                    };
                return result;
            }
            return null;
        }

        public static IExtensionsLogDetailWrapper Warp(this ExtensionsLogDetail eld)
        {
            if (eld != null)
            {
                var result = new ExtensionsLogDetailWrapper
                {
                    Name = eld.Name,
                    Assembly = eld.Assembly
                };
                return result;
            }
            return null;
        }

        public static IIndexStatsWrapper Wrap(this IndexStats indexStats)
        {
            if (indexStats != null)
            {
                var indexStatsWrapper = new IndexStatsWrapper
                {
                    Name = indexStats.Name,
                    IndexingAttempts = indexStats.IndexingAttempts,
                    IndexingSuccesses = indexStats.IndexingSuccesses,
                    IndexingErrors = indexStats.IndexingErrors,
                    LastIndexedEtag = indexStats.LastIndexedEtag.Wrap(),
                    LastIndexedTimestamp = indexStats.LastIndexedTimestamp,
                    LastQueryTimestamp = indexStats.LastQueryTimestamp,
                    TouchCount = indexStats.TouchCount,
                    Priority = (IndexingPriorityWrapper)indexStats.Priority,
                    ReduceIndexingAttempts = indexStats.ReduceIndexingAttempts,
                    ReduceIndexingSuccesses = indexStats.ReduceIndexingSuccesses,
                    ReduceIndexingErrors = indexStats.ReduceIndexingErrors,
                    LastReducedEtag = indexStats.LastReducedEtag.Wrap(),
                    LastReducedTimestamp = indexStats.LastReducedTimestamp,
                    CreatedTimestamp = indexStats.CreatedTimestamp,
                    LastIndexingTime = indexStats.LastIndexingTime,
                    IsOnRam = indexStats.IsOnRam,
                    LockMode = (IndexLockModeWrapper)indexStats.LockMode,
                    ForEntityName = indexStats.ForEntityName,
                    Performance = indexStats.Performance.Select(ips => ips.Wrap()).ToArray(),
                    DocsCount = indexStats.DocsCount
                };
                return indexStatsWrapper;
            }
            return null;
        }

        public static IAttachmentWrapper Wrap(this Attachment attachment)
        {
            if (attachment != null)
            {
                var attachemntWrapper = new AttachmentWrapper
                {
                    Data = attachment.Data,
                    Size = attachment.Size,
                    Metadata = attachment.Metadata.Wrap(),
                    Etag = attachment.Etag.Wrap(),
                    Key = attachment.Key
                };
                return attachemntWrapper;
            }
            return null;
        }

        public static FieldIndexingWrapper Wrap(this FieldIndexing fieldIndexing)
        {
            switch (fieldIndexing)
            {
                case FieldIndexing.Analyzed:
                    return FieldIndexingWrapper.Analyzed;
                case FieldIndexing.Default:
                    return FieldIndexingWrapper.Default;
                case FieldIndexing.No:
                    return FieldIndexingWrapper.No;
                case FieldIndexing.NotAnalyzed:
                    return FieldIndexingWrapper.NotAnalyzed;
                default:
                    throw new ArgumentException("Unknown FieldIndexing : " + fieldIndexing.ToString());
            }
        }

        public static FieldTermVectorWrapper Wrap(this FieldTermVector unwrapped)
        {
            switch (unwrapped)
            {
                case FieldTermVector.Yes:
                    return FieldTermVectorWrapper.Yes;
                case FieldTermVector.WithPositionsAndOffsets:
                    return FieldTermVectorWrapper.WithPositionsAndOffsets;
                case FieldTermVector.WithPositions:
                    return FieldTermVectorWrapper.WithPositions;
                case FieldTermVector.WithOffsets:
                    return FieldTermVectorWrapper.WithOffsets;
                case FieldTermVector.No:
                    return FieldTermVectorWrapper.No;
                default:
                    throw new ArgumentException("Unknown FieldTermVector : " + unwrapped.ToString());
            }
        }

        public static SortOptionsWrapper Wrap(this SortOptions unwrapped)
        {
            return (SortOptionsWrapper) ((int) unwrapped);
        }

        public static SpatialSearchStrategyWrapper Wrap(this SpatialSearchStrategy unwrapped)
        {
            switch (unwrapped)
            {
                case SpatialSearchStrategy.BoundingBox:
                    return SpatialSearchStrategyWrapper.BoundingBox;
                case SpatialSearchStrategy.GeohashPrefixTree:
                    return SpatialSearchStrategyWrapper.GeohashPrefixTree;
                case SpatialSearchStrategy.QuadPrefixTree:
                    return SpatialSearchStrategyWrapper.QuadPrefixTree;
                default:
                    throw new ArgumentException("Unknown SpatialSearchStrategy : " + unwrapped.ToString());
            }
        }

        public static SpatialFieldTypeWrapper Wrap(this SpatialFieldType unwrapped)
        {
            switch (unwrapped)
            {
                case SpatialFieldType.Cartesian:
                    return SpatialFieldTypeWrapper.Cartesian;
                case SpatialFieldType.Geography:
                    return SpatialFieldTypeWrapper.Geography;
                default:
                    throw new ArgumentException("Unknown SpatialFieldType : " + unwrapped.ToString());
            }
        }

        public static SpatialUnitsWrapper Wrap(this SpatialUnits unwrapped)
        {
            switch (unwrapped)
            {
                case SpatialUnits.Kilometers:
                    return SpatialUnitsWrapper.Kilometers;
                case SpatialUnits.Miles:
                    return SpatialUnitsWrapper.Miles;
                default:
                    throw new ArgumentException("Unknown SpatialUnits : " + unwrapped.ToString());
            }
        }

        public static ISpatialOptionsWrapper Wrap(this SpatialOptions unwrapped)
        {
            if (unwrapped != null)
            {
                var wrapper = new SpatialOptionsWrapper
                    {
                        MaxTreeLevel = unwrapped.MaxTreeLevel,
                        MaxX = unwrapped.MaxX,
                        MaxY = unwrapped.MaxY,
                        MinX = unwrapped.MinX,
                        MinY = unwrapped.MinY,
                        Strategy = unwrapped.Strategy.Wrap(),
                        Type = unwrapped.Type.Wrap(),
                        Units = unwrapped.Units.Wrap()
                    };
                return wrapper;
            }
            return null;
        }

        public static StringDistanceTypesWrapper Wrap(this StringDistanceTypes unwrapped)
        {
            switch (unwrapped)
            {
                case StringDistanceTypes.Default:
                    return StringDistanceTypesWrapper.Default;
                case StringDistanceTypes.JaroWinkler:
                    return StringDistanceTypesWrapper.JaroWinkler;
                case StringDistanceTypes.Levenshtein:
                    return StringDistanceTypesWrapper.Levenshtein;
                case StringDistanceTypes.NGram:
                    return StringDistanceTypesWrapper.NGram;
                case StringDistanceTypes.None:
                    return StringDistanceTypesWrapper.None;
                default:
                    throw new ArgumentException("Unknown StringDistanceTypes : " + unwrapped.ToString());
            }
        }

        public static ISuggestionOptionsWrapper Wrap(this SuggestionOptions unwrapped)
        {
            if (unwrapped != null)
            {
                var wrapper = new SuggestionOptionsWrapper
                    {
                        Accuracy = unwrapped.Accuracy,
                        Distance = unwrapped.Distance.Wrap()
                    };
                return wrapper;
            }
            return null;
        }

        public static IIndexDefinitionWrapper Wrap(this IndexDefinition unwrapped)
        {
            if (unwrapped != null)
            {
                var wrapper = new IndexDefinitionWrapper
                    {
                        Name = unwrapped.Name,
                        Maps = unwrapped.Maps,
                        Reduce = unwrapped.Reduce,
                        IsCompiled = unwrapped.IsCompiled,
                        Stores = unwrapped.Stores.ToDictionary(x => x.Key, y => y.Value == FieldStorage.Yes ? FieldStorageWrapper.Yes : FieldStorageWrapper.No),
                        Indexes = unwrapped.Indexes.ToDictionary(x => x.Key, y => y.Value.Wrap()),
                        SortOptions = unwrapped.SortOptions.ToDictionary(x => x.Key, y => y.Value.Wrap()),
                        Analyzers = unwrapped.Analyzers.ToDictionary(x => x.Key, y => y.Value),
                        Fields = new List<string>(unwrapped.Fields),
                        Suggestions = unwrapped.Suggestions.ToDictionary(x => x.Key, y => y.Value.Wrap()),
                        TermVectors = unwrapped.TermVectors.ToDictionary(x => x.Key, y => y.Value.Wrap()),
                        SpatialIndexes = unwrapped.SpatialIndexes.ToDictionary(x => x.Key, y => y.Value.Wrap()),
                        InternalFieldsMapping = unwrapped.InternalFieldsMapping.ToDictionary(x => x.Key, y => y.Value)
                    };
            }
            return null;
        }
    }
}