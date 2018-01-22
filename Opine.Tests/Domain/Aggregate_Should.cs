using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opine.Domain;

namespace Opine.Tests.Domain
{
    [TestClass]
    public class Aggregate_Should
    {
        public class TestMethodCalled : IEvent { }

        public class TestAggregate : Aggregate<int>
        {
            public TestAggregate() : base(0, 1)
            {
            }

            public void TestMethod()
            {
                Emit(new TestMethodCalled());
            }
        }

        [TestMethod]
        public void EmitEvents()
        {
            var aggregate = new TestAggregate();
            aggregate.TestMethod();
            Assert.AreEqual(1, aggregate.Events.Count());
            aggregate.TestMethod();
            Assert.AreEqual(2, aggregate.Events.Count());
        }

        [TestMethod]
        public void ReturnCorrectVersion()
        {
            var aggregate = new TestAggregate();
            Assert.AreEqual(1L, aggregate.Version);
        }
    }
}