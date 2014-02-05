using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.WrapperInterfaces;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class DatabaseStatisticsWrapper : IDatabaseStatisticsWrapper
    {
        public EtagWrapper LastDocEtag { get; set; }

        public EtagWrapper LastAttachmentEtag { get; set; }

        public int CountOfIndexes { get; set; }

        public int InMemoryIndexingQueueSize { get; set; }

        public long ApproximateTaskCount { get; set; }

        public long CountOfDocuments { get; set; }

        public string[] StaleIndexes { get; set; }

        public int CurrentNumberOfItemsToIndexInSingleBatch { get; set; }

        public int CurrentNumberOfItemsToReduceInSingleBatch { get; set; }

        public decimal DatabaseTransactionVersionSizeInMB { get; set; }

        public IIndexStatsWrapper[] Indexes { get; set; }

        public IServerErrorWrapper[] Errors { get; set; }

        public ITriggerInfoWrapper[] Triggers { get; set; }

        public IEnumerable<IExtensionsLogWrapper> Extensions { get; set; }

        public IActualIndexingBatchSizeWrapper[] ActualIndexingBatchSize { get; set; }

        public IFutureBatchStatsWrapper[] Prefetches { get; set; }

        public Guid DatabaseId { get; set; }
    }

    public class ActualIndexingBatchSizeWrapper : IActualIndexingBatchSizeWrapper
    {
        public int Size { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class FutureBatchStatsWrapper : IFutureBatchStatsWrapper
    {
        public DateTime Timestamp { get; set; }
        public TimeSpan? Duration { get; set; }
        public int? Size { get; set; }
        public int Retries { get; set; }
    }

    public class ExtensionsLogWrapper : IExtensionsLogWrapper
    {
        public string Name { get; set; }
        public IExtensionsLogDetailWrapper[] Installed { get; set; }
    }

    public class ExtensionsLogDetailWrapper : IExtensionsLogDetailWrapper
    {
        public string Name { get; set; }
        public string Assembly { get; set; }
    }
}
