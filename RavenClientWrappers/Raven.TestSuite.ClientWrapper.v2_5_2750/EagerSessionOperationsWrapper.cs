namespace Raven.TestSuite.ClientWrapper.v2_5_2750
{
    using Raven.Client.Document.Batches;
    using Raven.TestSuite.Common.WrapperInterfaces;

    public class EagerSessionOperationsWrapper : IEagerSessionOperationsWrapper
    {
        private readonly IEagerSessionOperations eagerSessionOperations;

        public EagerSessionOperationsWrapper(IEagerSessionOperations eagerSessionOperations)
        {
            this.eagerSessionOperations = eagerSessionOperations;
        }

        public void ExecuteAllPendingLazyOperations()
        {
            this.eagerSessionOperations.ExecuteAllPendingLazyOperations();
        }
    }
}
