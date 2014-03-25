namespace Raven.TestSuite.Common.WrapperInterfaces
{
    public interface IEagerSessionOperationsWrapper
    {
        void ExecuteAllPendingLazyOperations();
    }
}
