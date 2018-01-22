using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Opine.Serialization;
using Opine.Snapshots;

namespace Opine.Messaging.GetEventStore
{
    public class ESSnapshotStore : ISnapshotStore
    {
        private readonly IEventStoreConnection connection;

        public ESSnapshotStore(IEventStoreConnection connection)
        {
            this.connection = connection;
        }

        public async Task<Snapshot> Read(Type type, object id)
        {
            var streamName = GetStreamName(type, id);
            var messages = await connection.ReadStreamEventsBackwardAsync(streamName,
                0, 1, true, null);
            var message = messages.Events.FirstOrDefault();
            if (message.IsResolved)
            {
                var snapshot = (Snapshot)Serializer.ToObject(message.Event?.Data);
                return snapshot;
            }
            return null;
        }

        public async Task Store(Snapshot snapshot)
        {
            var type = snapshot.SnapshotData.GetType();
            var streamName = GetStreamName(type, snapshot.Id);
            await connection.AppendToStreamAsync(streamName, StreamVersion.Any, 
                new EventData(
                    Guid.NewGuid(),
                    type.Name,
                    true, 
                    Serializer.ToByteArray(snapshot),
                    null));

        }

        private static string GetStreamName(Type type, object id)
        {
            return new Stream(Categories.Snapshots, type, id).ToString();
        }
    }
}