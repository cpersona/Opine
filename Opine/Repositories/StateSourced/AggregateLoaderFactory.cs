using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Opine.Repositories.StateSourced
{
    public class AggregateLoaderFactory : IAggregateLoaderFactory
    {
        private IServiceProvider serviceProvider;
        private AggregateLoaderMap aggregateLoaderMap;

        public AggregateLoaderFactory(IServiceProvider serviceProvider, AggregateLoaderMap aggregateLoaderMap)
        {
            this.serviceProvider = serviceProvider;
            this.aggregateLoaderMap = aggregateLoaderMap;
        }

        public IAggregateLoader GetLoader(Type type)
        {
            Type t = aggregateLoaderMap.GetLoader(type);
            if (t != null)
            {
                return (IAggregateLoader)serviceProvider.GetRequiredService(t);
            }
            throw new Exception(""); // TODO: Exception
        }
    }
}