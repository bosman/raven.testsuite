using System.Linq;
using Raven.Abstractions.Data;
using Raven.Json.Linq;
using Raven.TestSuite.Common.Abstractions.Data;
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
    }
}