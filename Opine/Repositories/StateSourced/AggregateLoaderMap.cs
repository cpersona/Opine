using System;
using System.Collections.Generic;

namespace Opine.Repositories.StateSourced
{
    public class AggregateLoaderMap
    {
        private Dictionary<Type, Type> map = new Dictionary<Type, Type>();

        public void Add(Type aggregate, Type loader)
        {
            map.Add(aggregate, loader);
        }

        public Type GetLoader(Type aggregate)
        {
            Type loader = null;
            map.TryGetValue(aggregate, out loader);
            return loader;
        }
    }
}