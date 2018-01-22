using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opine.Dispatching;
using Opine.Dispatching.Static;
using Opine.Domain;

namespace Opine.Tests.Dispatching.Static
{
    [TestClass]
    public class HandlerFinder_Should
    {
        public class NonHandler
        {

        }

        public class TestEvent1 : IEvent { }

        public class TestEvent2 : IEvent { }

        public class TestHandler : IHandler, IHandler<TestEvent1>, IHandler<TestEvent2>
        {
            public Task Handle(MessageContext messageContext, TestEvent1 message)
            {
                throw new System.NotImplementedException();
            }

            public Task Handle(MessageContext messageContext, TestEvent2 message)
            {
                throw new System.NotImplementedException();
            }

            public Task Handle(MessageContext messageContext, object message)
            {
                throw new System.NotImplementedException();
            }
        }

        public class InvalidTestHandler : IHandler<TestEvent2>
        {
            public Task Handle(MessageContext messageContext, TestEvent2 message)
            {
                throw new System.NotImplementedException();
            }
        }

        [TestMethod]
        public void NotFindHandlers()
        {
            var finder = new StaticHandlerFinder();
            var handlers = finder.FindHandlers(typeof(NonHandler));
            Assert.AreEqual(0, handlers.Count());
            handlers = finder.FindHandlers(typeof(InvalidTestHandler));
            Assert.AreEqual(0, handlers.Count());
        }

        [TestMethod]
        public void FindHandlers()
        {
            var finder = new StaticHandlerFinder();
            var handlers = finder.FindHandlers(typeof(TestHandler));
            Assert.AreEqual(2, handlers.Count());
            Assert.AreEqual(typeof(TestEvent1), handlers.ToArray()[0].HandlerType);
            Assert.AreEqual(typeof(TestEvent2), handlers.ToArray()[1].HandlerType);
        }
    }
}