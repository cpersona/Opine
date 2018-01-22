using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opine.Dispatching;
using Opine.Dispatching.Dynamic;

namespace Opine.Tests.Dispatching.Dynamic
{
    public class Test
    {
        public void TestMethod(string x, int y) { }
    }

    [TestClass]
    public class HandlerInfo_Should
    {
        [TestMethod]
        public void ReturnParameterTypes()
        {
            var mi = typeof(Test).GetMethods()[0];
            var info = new HandlerInfo(mi);
            Assert.AreEqual(mi, info.MethodInfo);
            var parms = info.ParameterTypes;
            Assert.AreEqual(2, parms.Count());
            Assert.AreEqual(typeof(string), parms[0]);
            Assert.AreEqual(typeof(int), parms[1]);
        }
    }
}