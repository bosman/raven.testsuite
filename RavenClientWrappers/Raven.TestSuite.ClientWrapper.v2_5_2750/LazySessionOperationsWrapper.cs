namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    using Raven.Client.Document.Batches;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using System;
    using System.Collections.Generic;

    public class LazySessionOperationsWrapper : ILazySessionOperationsWrapper
    {
        private readonly ILazySessionOperations lazySessionOperations;

        public LazySessionOperationsWrapper(ILazySessionOperations lazySessionOperations)
        {
            this.lazySessionOperations = lazySessionOperations;
        }

        public Lazy<TResult[]> Load<TResult>(IEnumerable<string> ids)
        {
            return this.lazySessionOperations.Load<TResult>(ids);
        }

        public Lazy<TResult[]> Load<TResult>(IEnumerable<ValueType> ids)
        {
            return this.lazySessionOperations.Load<TResult>(ids);
        }

        public Lazy<TResult[]> Load<TResult>(params string[] ids)
        {
            return this.lazySessionOperations.Load<TResult>(ids);
        }

        public Lazy<TResult[]> Load<TResult>(params ValueType[] ids)
        {
            return this.lazySessionOperations.Load<TResult>(ids);
        }

        public Lazy<TResult> Load<TResult>(string id)
        {
            return this.lazySessionOperations.Load<TResult>(id);
        }

        public Lazy<TResult> Load<TResult>(ValueType id)
        {
            return this.lazySessionOperations.Load<TResult>(id);
        }

        public Lazy<TResult[]> Load<TResult>(IEnumerable<string> ids, Action<TResult[]> onEval)
        {
            return this.lazySessionOperations.Load<TResult>(ids, onEval);
        }

        public Lazy<TResult[]> Load<TResult>(IEnumerable<ValueType> ids, Action<TResult[]> onEval)
        {
            return this.lazySessionOperations.Load<TResult>(ids, onEval);
        }

        public Lazy<TResult> Load<TResult>(string id, Action<TResult> onEval)
        {
            return this.lazySessionOperations.Load<TResult>(id, onEval);
        }

        public Lazy<TResult> Load<TResult>(ValueType id, Action<TResult> onEval)
        {
            return this.lazySessionOperations.Load<TResult>(id, onEval);
        }

        public Lazy<TResult[]> LoadStartingWith<TResult>(string keyPrefix, string matches = null, int start = 0, int pageSize = 25, string exclude = null)
        {
            return this.lazySessionOperations.LoadStartingWith<TResult>(keyPrefix, matches, start, pageSize, exclude);
        }
    }
}
