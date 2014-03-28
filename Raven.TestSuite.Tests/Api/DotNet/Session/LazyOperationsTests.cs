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
        const string order1IdString = "orders/1";
        const int order1Id = 1;
        const string order2IdString = "orders/2";
        const int order2Id = 2;

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
                        Lazy<Order> lazyOrder = session.Advanced.Lazily.Load<Order>(order1IdString);
                        Assert.False(lazyOrder.IsValueCreated);
                        var order = lazyOrder.Value;
                        Assert.Equal(order1IdString, order.Id);

                        lazyOrder = session.Advanced.Lazily.Load<Order>(order1Id);
                        Assert.False(lazyOrder.IsValueCreated);
                        order = lazyOrder.Value;
                        Assert.Equal(order1IdString, order.Id);

                        Lazy<Order[]> lazyOrders = session.Advanced.Lazily.Load<Order>(new String[] { order1IdString, order2IdString });
                        Assert.False(lazyOrders.IsValueCreated);
                        Order[] orders = lazyOrders.Value;
                        Assert.Equal(2, orders.Length);
                        Assert.Equal(order1IdString, orders[0].Id);
                        Assert.Equal(order2IdString, orders[1].Id);

                        lazyOrders = session.Advanced.Lazily.Load<Order>(order1IdString, order2IdString);
                        Assert.False(lazyOrders.IsValueCreated);
                        orders = lazyOrders.Value;
                        Assert.Equal(2, orders.Length);
                        Assert.Equal(order1IdString, orders[0].Id);
                        Assert.Equal(order2IdString, orders[1].Id);

                        lazyOrders = session.Advanced.Lazily.Load<Order>(order1Id, order2Id);
                        Assert.False(lazyOrders.IsValueCreated);
                        orders = lazyOrders.Value;
                        Assert.Equal(2, orders.Length);
                        Assert.Equal(order1IdString, orders[0].Id);
                        Assert.Equal(order2IdString, orders[1].Id);
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

                        session.Advanced.Lazily.Load<Order>(order1IdString, x => order1 = x);
                        session.Advanced.Lazily.Load<Order>(order2IdString, x => order2 = x);
                        Assert.Null(order1);
                        Assert.Null(order2);

                        session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();
                        Assert.NotNull(order1);
                        Assert.NotNull(order2);
                        Assert.Equal(order1IdString, order1.Id);
                        Assert.Equal(order2IdString, order2.Id);
                    }
                }
            });
        }
    }
}
