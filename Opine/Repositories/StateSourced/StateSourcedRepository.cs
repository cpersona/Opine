using System;
using System.Linq;
using System.Threading.Tasks;
using Opine.Dispatching;
using Opine.Domain;
using Opine.Messaging;

namespace Opine.Repositories.StateSourced
{
    public class StateSourcedRepository : IRepository
    {
        private IAggregateLoaderFactory aggregateLoaderFactory;
        private IMessageStore messageStore;

        public StateSourcedRepository(IAggregateLoaderFactory aggregateLoaderFactory, IMessageStore messageStore)
        {
            this.aggregateLoaderFactory = aggregateLoaderFactory;
            this.messageStore = messageStore;
        }

        public async Task<IAggregate> Load(Type type, object id)
        {
            var loader = GetLoader(type);
            var aggregate = await loader.Load(type, id);
            return aggregate;
        }

        public async Task<TAggregate> Load<TAggregate>(object id) where TAggregate : IAggregate
        {
            return (TAggregate)await Load(typeof(TAggregate), id);
        }

        public async Task Save(MessageContext messageContext, IAggregate aggregate)
        {
            var stream = new Stream(Categories.Events, aggregate.GetType(), messageContext.AggregateId);
            await messageStore.Store(stream, StreamVersion.Any, 
                aggregate.Events.Select(x => 
                    new StorableMessage(
                        new Metadata(messageContext.AggregateId, 
                            messageContext.ProcessCode,
                            messageContext.ProcessId),
                        x)));
        }

        private IAggregateLoader GetLoader(Type type)
        {
            var loader = aggregateLoaderFactory.GetLoader(type);
            if (loader == null)
                throw new Exception(""); // TODO: Correct exception
            return loader;
        }
    }
}