using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opine.Dispatching;
using Opine.Dispatching.Dynamic;

namespace Opine.Tests.Dispatching.Dynamic
{
    [TestClass]
    public class HandlerRegistry_Should
    {
        public class Test
        {
            public void Method1() { }
            public void Method2() { }
        }

        [TestMethod]
        public void RegisterHandlers()
        {
            var methods = typeof(Test).GetMethods();
            var info1 = new HandlerInfo(methods[0]);
            var info2 = new HandlerInfo(methods[1]);
            var registry = new HandlerRegistry();
            registry.Register(typeof(string), info1);
            registry.Register(typeof(int), info2);
            var handlers1 = registry.GetHandlers(typeof(string));
            Assert.AreEqual(1, handlers1.Count());
            Assert.AreEqual(info1, handlers1.First());
            var handlers2 = registry.GetHandlers(typeof(int));
            Assert.AreEqual(1, handlers2.Count());
            Assert.AreEqual(info2, handlers2.First());
        }
    }
}