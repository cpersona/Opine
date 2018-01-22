using System;

namespace Opine.Repositories.StateSourced
{
    public interface IAggregateLoaderFactory
    {
         IAggregateLoader GetLoader(Type type);
    }
}