using System;
using System.Threading.Tasks;
using Opine.Dispatching;
using Opine.Domain;

namespace Opine.Repositories.StateSourced
{
    public interface IAggregateLoader
    {
        Task<IAggregate> Load(Type type, object id);
    }

    public interface IAggregateLoader<TAggregate> : IAggregateLoader
        where TAggregate : IAggregate
    {
         Task<TAggregate> Load(object id);
    }
}