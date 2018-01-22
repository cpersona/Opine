using System;

namespace Opine.Messaging
{
    public class StorableMessage
    {
        public StorableMessage(Metadata metadata, object data)
        {
            MessageId = Guid.NewGuid();
            Metadata = metadata;
            Data = data;
        }

        public Guid MessageId { get; private set; }
        public Metadata Metadata { get; private set; }
        public object Data { get; private set; }
    }
}