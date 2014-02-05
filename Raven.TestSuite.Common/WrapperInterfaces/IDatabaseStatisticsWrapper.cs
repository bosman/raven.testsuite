using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common.Abstractions.Data;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IDatabaseStatisticsWrapper
    {
        EtagWrapper LastDocEtag { get; set; }
        EtagWrapper LastAttachmentEtag { get; set; }
        int CountOfIndexes { get; set; }
        int InMemoryIndexingQueueSize { get; set; }
        long ApproximateTaskCount { get; set; }

        long CountOfDocuments { get; set; }

        string[] StaleIndexes { get; set; }

        int CurrentNumberOfItemsToIndexInSingleBatch { get; set; }

        int CurrentNumberOfItemsToReduceInSingleBatch { get; set; }

        decimal DatabaseTransactionVersionSizeInMB { get; set; }

        IIndexStatsWrapper[] Indexes { get; set; }

        IServerErrorWrapper[] Errors { get; set; }

        ITriggerInfoWrapper[] Triggers { get; set; }

        IEnumerable<IExtensionsLogWrapper> Extensions { get; set; }

        IActualIndexingBatchSizeWrapper[] ActualIndexingBatchSize { get; set; }
        IFutureBatchStatsWrapper[] Prefetches { get; set; }

        Guid DatabaseId { get; set; }
    }

    public interface ITriggerInfoWrapper
    {
        string Type { get; set; }
        string Name { get; set; }
    }

    public interface IActualIndexingBatchSizeWrapper
    {
        int Size { get; set; }
        DateTime Timestamp { get; set; }
    }

    public interface IFutureBatchStatsWrapper
    {
        DateTime Timestamp { get; set; }
        TimeSpan? Duration { get; set; }
        int? Size { get; set; }
        int Retries { get; set; }
    }

    public interface IExtensionsLogWrapper
    {
        string Name { get; set; }
        IExtensionsLogDetailWrapper[] Installed { get; set; }
    }

    public interface IExtensionsLogDetailWrapper
    {
        string Name { get; set; }
        string Assembly { get; set; }
    }
}
