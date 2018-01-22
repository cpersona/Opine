using System;
using System.Threading.Tasks;
using Opine.Dispatching;
using Opine.Domain;

namespace Opine.Repositories
{
    public interface IRepository
    {
         Task<IAggregate> Load(Type type, object id);

         Task<TAggregate> Load<TAggregate>(object id) where TAggregate : IAggregate;

         Task Save(MessageContext messageContext, IAggregate aggregate);
    }
}