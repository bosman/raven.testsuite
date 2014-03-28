namespace Raven.TestSuite.Tests.Api.DotNet.Session
{
    using Raven.TestSuite.Common.Abstractions;
    using Raven.TestSuite.Common.WrapperInterfaces;
    using Raven.TestSuite.Tests.Api.Rest;
    using Raven.TestSuite.Tests.Common.Attributes;
    using Raven.TestSuite.Tests.DatabaseObjects.Northwind;
    using System;
    using Xunit;

    [Serializable]
    [RequiresFreshNorthwindDatabaseAttribute]
    public class LazyOperationsTests : BaseDotNetApiTestGroup
    {
        const string order1Id = "orders/1";
        const string order2Id = "orders/2";

        public LazyOperationsTests(IRavenClientWrapper wrapper)
            : base(wrapper)
        {
        }

        [RavenDotNetApiTest]
        public void LazilyLoadEntityTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        Lazy<Order> lazyOrder = session.Advanced.Lazily.Load<Order>(order1Id);
                        Assert.False(lazyOrder.IsValueCreated);
                        var order = lazyOrder.Value;
                        Assert.Equal(order1Id, order.Id);

                        Lazy<Order[]> lazyOrders = session.Advanced.Lazily.Load<Order>(new String[] { order1Id, order2Id });
                        Assert.False(lazyOrders.IsValueCreated);
                        Order[] orders = lazyOrders.Value;
                        Assert.Equal(2, orders.Length);
                        Assert.Equal(order1Id, orders[0].Id);
                        Assert.Equal(order2Id, orders[1].Id);
                    }
                }
            });
        }

        [RavenDotNetApiTest]
        public void ExecuteAllPendingLazyOperationsTest()
        {
            this.wrapper.Execute(env =>
            {
                using (var store = env.CreateDocumentStore(Constants.DbName.Northwind).Initialize())
                {
                    using (var session = store.OpenSession())
                    {
                        Order order1 = null;
                        Order order2 = null;

                        session.Advanced.Lazily.Load<Order>(order1Id, x => order1 = x);
                        session.Advanced.Lazily.Load<Order>(order2Id, x => order2 = x);
                        Assert.Null(order1);
                        Assert.Null(order2);

                        session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();
                        Assert.NotNull(order1);
                        Assert.NotNull(order2);
                        Assert.Equal(order1Id, order1.Id);
                        Assert.Equal(order2Id, order2.Id);
                    }
                }
            });
        }
    }
}
