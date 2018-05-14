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
        private IMetadataFactory metadataFactory;

        public StateSourcedRepository(
            IAggregateLoaderFactory aggregateLoaderFactory, 
            IMessageStore messageStore,
            IMetadataFactory metadataFactory)
        {
            this.aggregateLoaderFactory = aggregateLoaderFactory;
            this.messageStore = messageStore;
            this.metadataFactory = metadataFactory;
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
                aggregate.Events.Select(evt => 
                    new StorableMessage(
                        metadataFactory.Create(messageContext),
                        evt)));
        }

        private IAggregateLoader GetLoader(Type type)
        {
            var loader = aggregateLoaderFactory.GetLoader(type);
            if (loader == null)
                throw new Exception($"No loader found for type '{type.Name}'"); // TODO: Correct exception
            return loader;
        }
    }
}