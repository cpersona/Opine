using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Opine.Dispatching;
using Opine.Dispatching.Dynamic;
using Opine.Domain;
using Opine.Repositories;

namespace Opine.Tests.Dispatching.Dynamic
{
    [TestClass]
    public class DynamicDispatcher_Should
    {
        public interface ITestHandler
        {
            void Handle(MessageContext context, TestEvent e);
        }

        public class TestEvent : IEvent
        {

        }

        public class TestEvent2 : IEvent
        {

        }

        [TestMethod]
        public void CallHandler()
        {
            var registry = new Mock<IHandlerRegistry>();
            var mi = typeof(ITestHandler).GetMethod("Handle");
            registry
                .Setup(x => x.GetHandlers(typeof(TestEvent)))
                .Returns((IEnumerable<HandlerInfo>)new[] { new HandlerInfo(mi) });

            var handler = new Mock<ITestHandler>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(ITestHandler)))
                .Returns(handler.Object);

            var unitOfWork = new Mock<IUnitOfWork>();

            var dispatcher = new DynamicDispatcher(registry.Object, serviceProvider.Object, 
                unitOfWork.Object);

            var context = new MessageContext(Guid.NewGuid(), 123, "ABC", 123);
            var e = new TestEvent();
            dispatcher.Dispatch(context, e).Wait();

            handler.Verify(x => x.Handle(context, e));
            unitOfWork.Verify(x => x.Commit());
        }

        [TestMethod]
        public void NotCallHandler()
        {
            var registry = new Mock<IHandlerRegistry>();
            var mi = typeof(ITestHandler).GetMethod("Handle");
            registry
                .Setup(x => x.GetHandlers(typeof(TestEvent)))
                .Returns((IEnumerable<HandlerInfo>)new[] { new HandlerInfo(mi) });

            var handler = new Mock<ITestHandler>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(ITestHandler)))
                .Returns(handler.Object);

            var unitOfWork = new Mock<IUnitOfWork>();

            var dispatcher = new DynamicDispatcher(registry.Object, serviceProvider.Object, 
                unitOfWork.Object);

            var context = new MessageContext(Guid.NewGuid(), 123, "ABC", 123);
            var e = new TestEvent2();
            dispatcher.Dispatch(context, e).Wait();

            handler.Verify(x => x.Handle(context, It.IsAny<TestEvent>()), Times.Never);
            unitOfWork.Verify(x => x.Commit(), Times.Never);
        }
    }
}