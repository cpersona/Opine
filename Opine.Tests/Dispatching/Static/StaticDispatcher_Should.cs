using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Opine.Dispatching;
using Opine.Dispatching.Static;
using Opine.Domain;

namespace Opine.Tests.Dispatching.Static
{
    [TestClass]
    public class StaticDispatcher_Should
    {
        public class TestEvent : IEvent
        {
            
        }

        [TestMethod]
        public void CallHandler()
        {
            var handler = new Mock<IHandler>();
            var registry = new Mock<IHandlerRegistry>();
            registry
                .Setup(x => x.GetHandlers(typeof(TestEvent)))
                .Returns((IEnumerable<HandlerInfo>)new[] { new HandlerInfo(handler.GetType()) });
            var provider = new Mock<IServiceProvider>();
            provider
                .Setup(x => x.GetService(handler.GetType()))
                .Returns(handler.Object);
            var uow = new Mock<IUnitOfWork>();

            var dispatcher = new StaticDispatcher(registry.Object, provider.Object, uow.Object);
            var context = new MessageContext(Guid.NewGuid(), 123, "ABC", 123);
            var message = new TestEvent();
            dispatcher.Dispatch(context, message).Wait();

            handler.Verify(x => x.Handle(context, message), Times.Once);
            uow.Verify(x => x.Commit(), Times.Once);
        }
    }
}