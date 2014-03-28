namespace Raven.TestSuite.ClientWrapper.v2_5_2750.Extensions
{
    using Raven.Abstractions.Data;
    using Raven.Abstractions.Indexing;
    using Raven.Json.Linq;
    using Raven.TestSuite.Common.Abstractions.Data;
    using Raven.TestSuite.Common.Abstractions.Enums;
    using Raven.TestSuite.Common.Abstractions.Json.Linq;
    using Raven.TestSuite.Common.Data;
    using Raven.TestSuite.Common.Indexing;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        public static IndexingPerformanceStatsWrapper Wrap(this IndexingPerformanceStats ips)
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

        public static DatabaseStatisticsWrapper Wrap(this DatabaseStatistics databaseStatistics)
        {
            if (databaseStatistics != null)
            {
                var databaseStatisticsWrapper = new DatabaseStatisticsWrapper
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
                    Errors = databaseStatistics.Errors.Select(err => err.Wrap() as ServerErrorWrapper).ToArray(),
                    Triggers = databaseStatistics.Triggers.Select(t => t.Wrap() as ITriggerInfoWrapper).ToArray(),
                    Extensions = databaseStatistics.Extensions.Select(e => e.Wrap()).ToArray(),
                    ActualIndexingBatchSize = databaseStatistics.ActualIndexingBatchSize.Select(a => a.Wrap()).ToArray(),
                    Prefetches = databaseStatistics.Prefetches.Select(p => p.Wrap()).ToArray(),
                    DatabaseId = databaseStatistics.DatabaseId
                };
                return databaseStatisticsWrapper;
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

        public static IndexStatsWrapper Wrap(this IndexStats indexStats)
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

        public static AttachmentWrapper Wrap(this Attachment attachment)
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

        public static T ConvertEnum<T>(IConvertible source)
            where T : IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            if (!source.GetType().IsEnum)
            {
                throw new ArgumentException("source must be an enumerated type");
            }
            return (T)Enum.Parse(typeof(T), source.ToString());
        }

        public static SortOptionsWrapper Wrap(this SortOptions unwrapped)
        {
            return (SortOptionsWrapper) ((int) unwrapped);
        }

        public static SpatialOptionsWrapper Wrap(this SpatialOptions unwrapped)
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
                        Strategy = ConvertEnum<SpatialSearchStrategyWrapper>(unwrapped.Strategy),
                        Type = ConvertEnum<SpatialFieldTypeWrapper>(unwrapped.Type),
                        Units = ConvertEnum<SpatialUnitsWrapper>(unwrapped.Units)
                    };
                return wrapper;
            }
            return null;
        }

        public static SpatialOptions Unwrap(this SpatialOptionsWrapper wrapped)
        {
            if (wrapped != null)
            {
                var unwrapped = new SpatialOptions
                    {
                        MaxTreeLevel = wrapped.MaxTreeLevel,
                        MaxX = wrapped.MaxX,
                        MaxY = wrapped.MaxY,
                        MinX = wrapped.MinX,
                        MinY = wrapped.MinY,
                        Strategy = ConvertEnum<SpatialSearchStrategy>(wrapped.Strategy),
                        Type = ConvertEnum<SpatialFieldType>(wrapped.Type),
                        Units = ConvertEnum<SpatialUnits>(wrapped.Units)
                    };
                return unwrapped;
            }
            return null;
        }

        public static SuggestionOptionsWrapper Wrap(this SuggestionOptions unwrapped)
        {
            if (unwrapped != null)
            {
                var wrapper = new SuggestionOptionsWrapper
                    {
                        Accuracy = unwrapped.Accuracy,
                        Distance = ConvertEnum<StringDistanceTypesWrapper>(unwrapped.Distance)
                    };
                return wrapper;
            }
            return null;
        }

        public static SuggestionOptions Unwrap(this SuggestionOptionsWrapper wrapped)
        {
            if (wrapped != null)
            {
                var unwrapped = new SuggestionOptions
                    {
                        Accuracy = wrapped.Accuracy,
                        Distance = ConvertEnum<StringDistanceTypes>(wrapped.Distance)
                    };
                return unwrapped;
            }
            return null;
        }

        public static IndexDefinitionWrapper Wrap(this IndexDefinition unwrapped)
        {
            if (unwrapped != null)
            {
                var wrapped = new IndexDefinitionWrapper
                    {
                        Name = unwrapped.Name,
                        Maps = unwrapped.Maps,
                        Reduce = unwrapped.Reduce,
                        IsCompiled = unwrapped.IsCompiled,
                        Stores = unwrapped.Stores.ToDictionary(x => x.Key, y => y.Value == FieldStorage.Yes ? FieldStorageWrapper.Yes : FieldStorageWrapper.No),
                        Indexes = unwrapped.Indexes.ToDictionary(x => x.Key, y => ConvertEnum<FieldIndexingWrapper>(y.Value)),
                        SortOptions = unwrapped.SortOptions.ToDictionary(x => x.Key, y => y.Value.Wrap()),
                        Analyzers = unwrapped.Analyzers.ToDictionary(x => x.Key, y => y.Value),
                        Fields = new List<string>(unwrapped.Fields),
                        Suggestions = unwrapped.Suggestions.ToDictionary(x => x.Key, y => y.Value.Wrap()),
                        TermVectors = unwrapped.TermVectors.ToDictionary(x => x.Key, y => ConvertEnum<FieldTermVectorWrapper>(y.Value)),
                        SpatialIndexes = unwrapped.SpatialIndexes.ToDictionary(x => x.Key, y => y.Value.Wrap()),
                        InternalFieldsMapping = unwrapped.InternalFieldsMapping.ToDictionary(x => x.Key, y => y.Value)
                    };
                return wrapped;
            }
            return null;
        }

        public static IndexDefinition Unwrap(this IndexDefinitionWrapper wrapped)
        {
            if (wrapped != null)
            {
                var unwrapped = new IndexDefinition
                    {
                        Name = wrapped.Name,
                        Maps = wrapped.Maps,
                        Reduce = wrapped.Reduce,
                        IsCompiled = wrapped.IsCompiled,
                        Stores = wrapped.Stores.ToDictionary(x => x.Key, y => ConvertEnum<FieldStorage>(y.Value)),
                        Indexes = wrapped.Indexes.ToDictionary(x => x.Key, y => ConvertEnum<FieldIndexing>(y.Value)),
                        SortOptions = wrapped.SortOptions.ToDictionary(x => x.Key, y => ConvertEnum<SortOptions>(y.Value)),
                        Analyzers = wrapped.Analyzers.ToDictionary(x => x.Key, y => y.Value),
                        Fields = new List<string>(wrapped.Fields),
                        Suggestions = wrapped.Suggestions.ToDictionary(x => x.Key, y => y.Value.Unwrap()),
                        TermVectors = wrapped.TermVectors.ToDictionary(x => x.Key, y => ConvertEnum<FieldTermVector>(y.Value)),
                        SpatialIndexes = wrapped.SpatialIndexes.ToDictionary(x => x.Key, y => y.Value.Unwrap()),
                        InternalFieldsMapping = wrapped.InternalFieldsMapping.ToDictionary(x => x.Key, y => y.Value)
                    };
                return unwrapped;
            }
            return null;
        }

        public static ServerErrorWrapper Wrap(this ServerError unwrapped)
        {
            if (unwrapped != null)
            {
                var result = new ServerErrorWrapper
                    {
                        Index = unwrapped.Index,
                        Error = unwrapped.Error,
                        Timestamp = unwrapped.Timestamp,
                        Document = unwrapped.Document,
                        Action = unwrapped.Action
                    };
                return result;
            }
            return null;
        }

        public static ServerError Unrap(this ServerErrorWrapper wrapped)
        {
            if (wrapped != null)
            {
                var result = new ServerError
                {
                    Index = wrapped.Index,
                    Error = wrapped.Error,
                    Timestamp = wrapped.Timestamp,
                    Document = wrapped.Document,
                    Action = wrapped.Action
                };
                return result;
            }
            return null;
        }

        public static TriggerInfoWrapper Wrap(this Raven.Abstractions.Data.DatabaseStatistics.TriggerInfo unwrapped)
        {
            if (unwrapped != null)
            {
                var result = new TriggerInfoWrapper
                {
                    Name = unwrapped.Name,
                    Type = unwrapped.Type
                };
                return result;
            }
            return null;
        }

        public static Raven.Abstractions.Data.DatabaseStatistics.TriggerInfo Unwrap(this TriggerInfoWrapper wrapped)
        {
            if (wrapped != null)
            {
                var result = new Raven.Abstractions.Data.DatabaseStatistics.TriggerInfo
                {
                    Name = wrapped.Name,
                    Type = wrapped.Type
                };
                return result;
            }
            return null;
        }

        public static PutResultWrapper Wrap(this PutResult unwrapped)
        {
            if (unwrapped != null)
            {
                var result = new PutResultWrapper
                {
                    Key = unwrapped.Key,
                    ETag = unwrapped.ETag.Wrap()
                };
                return result;
            }
            return null;
        }

        public static PutResult Unrap(this PutResultWrapper wrapped)
        {
            if (wrapped != null)
            {
                var result = new PutResult
                {
                    Key = wrapped.Key,
                    ETag = wrapped.ETag.Unwrap()
                };
                return result;
            }
            return null;
        }
    }
}