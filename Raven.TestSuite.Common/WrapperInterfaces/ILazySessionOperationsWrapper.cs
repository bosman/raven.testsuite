namespace Raven.TestSuite.Common.WrapperInterfaces
{
    using System;
    using System.Collections.Generic;

    public interface ILazySessionOperationsWrapper
    {
        //TODO
        //ILazyLoaderWithInclude<TResult> Include<TResult>(Expression<Func<TResult, object>> path);
        //ILazyLoaderWithInclude<object> Include(string path);
        Lazy<TResult[]> Load<TResult>(IEnumerable<string> ids);
        Lazy<TResult[]> Load<TResult>(IEnumerable<ValueType> ids);
        Lazy<TResult[]> Load<TResult>(params string[] ids);
        //Lazy<TResult[]> Load<TTransformer, TResult>(params string[] ids) where TTransformer : Raven.Client.Indexes.AbstractTransformerCreationTask, new();
        Lazy<TResult[]> Load<TResult>(params ValueType[] ids);
        //Lazy<TResult> Load<TTransformer, TResult>(string id) where TTransformer : Raven.Client.Indexes.AbstractTransformerCreationTask, new();
        Lazy<TResult> Load<TResult>(string id);
        Lazy<TResult> Load<TResult>(ValueType id);
        Lazy<TResult[]> Load<TResult>(IEnumerable<string> ids, Action<TResult[]> onEval);
        Lazy<TResult[]> Load<TResult>(IEnumerable<ValueType> ids, Action<TResult[]> onEval);
        Lazy<TResult> Load<TResult>(string id, Action<TResult> onEval);
        Lazy<TResult> Load<TResult>(ValueType id, Action<TResult> onEval);
        Lazy<TResult[]> LoadStartingWith<TResult>(string keyPrefix, string matches = null, int start = 0, int pageSize = 25, string exclude = null);
    }
}
