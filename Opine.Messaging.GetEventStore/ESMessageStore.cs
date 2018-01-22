using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Opine.Serialization;

namespace Opine.Messaging.GetEventStore
{
    public class ESMessageStore : IMessageStore
    {
        private IEventStoreConnection connection;

        public ESMessageStore(IEventStoreConnection connection)
        {
            this.connection = connection;
        }

        public async Task<IEnumerable<StoredMessage>> Read(Stream stream, long position, int count)
        {
            var streamName = stream.ToString();
            var slice = await connection.ReadStreamEventsForwardAsync(streamName, 
                position, count, true, null);
            return slice.Events
                .Select(x => 
                    new StoredMessage(
                        x.Event.EventId,
                        stream,
                        (Metadata)Serializer.ToObject(x.Event.Metadata),
                        Serializer.ToObject(x.Event.Data),
                        x.Event.EventNumber));
        }

        public async Task Store(Stream stream, long version, IEnumerable<StorableMessage> storableMessages)
        {
            var streamName = stream.ToString();
            await connection.AppendToStreamAsync(streamName, version, 
                storableMessages
                    .Select(x => 
                        new EventData(
                            x.MessageId, 
                            x.Data.GetType().Name,
                            true,
                            Serializer.ToByteArray(x.Data),
                            Serializer.ToByteArray(x.Metadata))));
        }
    }
}