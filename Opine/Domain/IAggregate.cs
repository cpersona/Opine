using System.Collections.Generic;

namespace Opine.Domain
{
    public interface IAggregate
    {
        IEnumerable<IEvent> Events { get; }
        object RootObject { get; }
        long Version { get; }
    }

    public interface IAggregate<TRoot> : IAggregate
    {
        TRoot Root { get; }
    }
}