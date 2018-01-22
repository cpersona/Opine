using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opine.Dispatching;
using Opine.Domain;
using Opine.Messaging;
using Opine.Snapshots;

namespace Opine.Repositories.EventSourced
{
    public class EventSourcedRepository : IRepository
    {
        private const int READ_BUFFER_SIZE = 100;
        private IMessageStore messageStore;
        private ISnapshotStore snapshotStore;

        public EventSourcedRepository(IMessageStore messageStore)
        {
            this.messageStore = messageStore;
        }

        public async Task<IAggregate> Load(Type type, object id)
        {
            // Do we have a snapshot? Initialize the root from that
            var snapshot = await snapshotStore.Read(type, id);
            var position = snapshot?.Version ?? 0L;
            var root = snapshot?.SnapshotData;
            // Read events from the stream (starting at the snapshot's version)
            var stream = new Stream(Categories.Events, type, id);
            IEnumerable<StoredMessage> messages = null;
            var events = new List<IEvent>();
            do 
            {
                messages = await messageStore.Read(stream, position, READ_BUFFER_SIZE);
                events.AddRange(messages.Select(x => (IEvent)x.Data));
                position += messages.Count();
            }
            while (messages.Count() == READ_BUFFER_SIZE);
            // Instantiate the aggregate
            return (IAggregate)Activator.CreateInstance(type, root, position, events);
        }

        public async Task<TAggregate> Load<TAggregate>(object id) where TAggregate : IAggregate
        {
            return (TAggregate)(await Load(typeof(TAggregate), id));
        }

        public async Task Save(MessageContext messageContext, IAggregate aggregate)
        {
            var storables = aggregate.Events
                .Select(x => 
                    new StorableMessage(
                        new Metadata(messageContext.AggregateId, messageContext.ProcessCode, messageContext.ProcessId),
                        x));
            var stream = new Stream(Categories.Events, aggregate.GetType(), messageContext.AggregateId);
            await messageStore.Store(stream, aggregate.Version, storables);
            // TODO: Try catch 
            await snapshotStore.Store(
                new Snapshot(messageContext.AggregateId, aggregate.Version, aggregate.RootObject));
        }
    }
}