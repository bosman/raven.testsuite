using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Common.Abstractions.Data;

namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IIndexStatsWrapper
    {
        string Name { get; set; }
        int IndexingAttempts { get; set; }
        int IndexingSuccesses { get; set; }
        int IndexingErrors { get; set; }
        EtagWrapper LastIndexedEtag { get; set; }
        DateTime LastIndexedTimestamp { get; set; }
        DateTime? LastQueryTimestamp { get; set; }
        int TouchCount { get; set; }
        IndexingPriorityWrapper Priority { get; set; }
        int? ReduceIndexingAttempts { get; set; }
        int? ReduceIndexingSuccesses { get; set; }
        int? ReduceIndexingErrors { get; set; }
        EtagWrapper LastReducedEtag { get; set; }
        DateTime? LastReducedTimestamp { get; set; }
        DateTime CreatedTimestamp { get; set; }
        DateTime LastIndexingTime { get; set; }
        string IsOnRam { get; set; }
        IndexLockModeWrapper LockMode { get; set; }
        List<string> ForEntityName { get; set; }

        IIndexingPerformanceStatsWrapper[] Performance { get; set; }
        int DocsCount { get; set; }
    }

    public interface IIndexingPerformanceStatsWrapper
    {
        bool Equals(IIndexingPerformanceStatsWrapper other);

        string Operation { get; set; }
        int OutputCount { get; set; }
        int InputCount { get; set; }
        int ItemsCount { get; set; }
        TimeSpan Duration { get; set; }
        DateTime Started { get; set; }
        double DurationMilliseconds { get; }
    }
}
