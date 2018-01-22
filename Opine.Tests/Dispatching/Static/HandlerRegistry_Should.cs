using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opine.Dispatching.Static;

namespace Opine.Tests.Dispatching.Static
{
    [TestClass]
    public class HandlerRegistry_Should
    {
        [TestMethod]
        public void RegisterHandlers()
        {
            var registry = new HandlerRegistry();
            registry.Register(typeof(string), new StaticHandlerInfo(typeof(string)));
            registry.Register(typeof(string), new StaticHandlerInfo(typeof(float)));
            registry.Register(typeof(int), new StaticHandlerInfo(typeof(int)));

            var handlers = registry.GetHandlers(typeof(string));
            Assert.AreEqual(2, handlers.Count());
            Assert.AreEqual(typeof(string), handlers.ToArray()[0].Type);
            Assert.AreEqual(typeof(float), handlers.ToArray()[1].Type);
            handlers = registry.GetHandlers(typeof(int));
            Assert.AreEqual(1, handlers.Count());
            Assert.AreEqual(typeof(int), handlers.ToArray()[0].Type);
            handlers = registry.GetHandlers(typeof(float));
            Assert.AreEqual(0, handlers.Count());
        }
    }
}