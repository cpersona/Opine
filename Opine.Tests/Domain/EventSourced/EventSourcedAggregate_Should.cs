using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Opine.Domain;
using Opine.Domain.EventSourced;

namespace Opine.Tests.Domain.EventSourced
{
    [TestClass]
    public class EventSourcedAggregate_Should
    {
        public class TestEvent : IEvent { }
        public class TestAggregate : EventSourcedAggregate<int>
        {
            public TestAggregate(int root, long version) : base(root, version)
            {
                When<TestEvent>(Do);
            }

            private void Do(TestEvent arg2)
            {
                InvokeCount++;
            }

            public void TestMethod()
            {
                Emit(new TestEvent());
            }

            public int InvokeCount { get; private set; }
        }

        [TestMethod]
        public void CallEventApplicatorAndEmitEvent()
        {
            var aggregate = new TestAggregate(1, 1);
            aggregate.TestMethod();
            Assert.AreEqual(1, aggregate.Events.Count());
            Assert.IsInstanceOfType(aggregate.Events.First(), typeof(TestEvent));
            Assert.AreEqual(1, aggregate.InvokeCount);
        }
    }
}