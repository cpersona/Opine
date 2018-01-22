using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opine.Messaging
{
    public class CachedMessageStore : IMessageStore
    {
        private class CachedStorableMessage
        {
            public CachedStorableMessage(Stream stream, long version, IEnumerable<StorableMessage> storableMessages)
            {
                Stream = stream;
                Version = version;
                StorableMessages = storableMessages;
            }

            public Stream Stream { get; private set; }
            public long Version { get; private set; }
            public IEnumerable<StorableMessage> StorableMessages { get; private set; }
        }

        private List<CachedStorableMessage> messages = 
            new List<CachedStorableMessage>();

        private IMessageStore messageStore;

        public CachedMessageStore(IMessageStore messageStore)
        {
            this.messageStore = messageStore;
        }

        public async Task<IEnumerable<StoredMessage>> Read(Stream stream, long position, int count)
        {
            return await messageStore.Read(stream, position, count);
        }

        public async Task Store(Stream stream, long version, IEnumerable<StorableMessage> storableMessages)
        {
            messages.Add(new CachedStorableMessage(stream, version, storableMessages));
        }

        public async Task SaveMessages()
        {
            foreach (var m in messages)
            {
                await messageStore.Store(m.Stream, m.Version, m.StorableMessages);
            }
            messages.Clear();
        }
    }
}