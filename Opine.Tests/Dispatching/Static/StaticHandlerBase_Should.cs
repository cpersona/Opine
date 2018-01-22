using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opine.Dispatching;
using Opine.Dispatching.Static;
using Opine.Domain;

namespace Opine.Tests.Dispatching.Static
{
    [TestClass]
    public class StaticHandlerBase_Should
    {
        public class TestEvent : IEvent { }

        public class TestEvent2 : IEvent { }

        public class TestHandler : StaticHandlerBase, IHandler<TestEvent>
        {
            public TestHandler()
            {
                Register<TestEvent>(Handle);
            }

            public async Task Handle(MessageContext messageContext, TestEvent message)
            {
                InvokeCount++;
            }

            public int InvokeCount { get; private set; }
        }

        [TestMethod]
        public void InvokeHandler()
        {
            var handler = new TestHandler();
            var context = new MessageContext(Guid.NewGuid(), 123, "ABC", 123);
            var message = new TestEvent();
            handler.Handle(context, message).Wait();
            Assert.AreEqual(1, handler.InvokeCount);
        }

        [TestMethod]
        public void NotInvokeHandler()
        {
            var handler = new TestHandler();
            var context = new MessageContext(Guid.NewGuid(), 123, "ABC", 123);
            var message = new TestEvent2();
            handler.Handle(context, message).Wait();
            Assert.AreEqual(0, handler.InvokeCount);
        }
    }
}