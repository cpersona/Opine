using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opine.Dispatching;
using Opine.Dispatching.Dynamic;
using Opine.Domain;

namespace Opine.Tests.Dispatching.Dynamic
{
    [TestClass]
    public class HandlerFactory_Should
    {
        public class TestEvent : IEvent
        {
            
        }

        public class TestHandler
        {
            public void Handle(MessageContext context, TestEvent e) { }

            public void Handle(MessageContext context) { }

            public void Handle(TestEvent e) { }

            public void Handle(int x) { }

            public void Handle() { }
        }

        [TestMethod]
        public void FindHandlers()
        {
            var finder = new HandlerFinder();
            var handlers = finder.FindHandlers(typeof(TestHandler));
            Assert.AreEqual(2, handlers.Count());
        }
    }
}