using Raven.Abstractions.Data;
using Raven.TestSuite.Common.Abstractions.Data;
using Raven.TestSuite.Common.WrapperInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Raven.TestSuite.ClientWrapper.v2_5_2750.Extensions;

namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    public class IndexStatsWrapper : IIndexStatsWrapper
    {
        public string Name { get; set; }

        public int IndexingAttempts { get; set; }

        public int IndexingSuccesses { get; set; }

        public int IndexingErrors { get; set; }

        public EtagWrapper LastIndexedEtag { get; set; }

        public DateTime LastIndexedTimestamp { get; set; }

        public DateTime? LastQueryTimestamp { get; set; }

        public int TouchCount { get; set; }

        public IndexingPriorityWrapper Priority { get; set; }

        public int? ReduceIndexingAttempts { get; set; }

        public int? ReduceIndexingSuccesses { get; set; }

        public int? ReduceIndexingErrors { get; set; }

        public EtagWrapper LastReducedEtag { get; set; }

        public DateTime? LastReducedTimestamp { get; set; }

        public DateTime CreatedTimestamp { get; set; }

        public DateTime LastIndexingTime { get; set; }

        public string IsOnRam { get; set; }

        public IndexLockModeWrapper LockMode { get; set; }

        public List<string> ForEntityName { get; set; }

        public IIndexingPerformanceStatsWrapper[] Performance { get; set; }

        public int DocsCount { get; set; }
    }

    public class IndexingPerformanceStatsWrapper : IIndexingPerformanceStatsWrapper
    {
        public bool Equals(IIndexingPerformanceStatsWrapper other)
        {
            return string.Equals(Operation, other.Operation) && OutputCount == other.OutputCount && InputCount == other.InputCount && Duration.Equals(other.Duration) && Started.Equals(other.Started);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Operation != null ? Operation.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ OutputCount;
                hashCode = (hashCode * 397) ^ InputCount;
                hashCode = (hashCode * 397) ^ Duration.GetHashCode();
                hashCode = (hashCode * 397) ^ Started.GetHashCode();
                return hashCode;
            }
        }

        public string Operation { get; set; }
        public int OutputCount { get; set; }
        public int InputCount { get; set; }
        public int ItemsCount { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime Started { get; set; }
        public double DurationMilliseconds { get { return Math.Round(Duration.TotalMilliseconds, 2); } }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IndexingPerformanceStats)obj);
        }

        public override string ToString()
        {
            return string.Format(@"
Operation:         {0}
Input:              {1:#,#}
Output:              {2:#,#}
Duration:          {3}
Duration in ms: {4:#,#}
", Operation,
                                 InputCount,
                                 OutputCount,
                                 Duration,
                                 DurationMilliseconds);

        }
    }
}
