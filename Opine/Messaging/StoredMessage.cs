using System;

namespace Opine.Messaging
{
    public class StoredMessage
    {
        public StoredMessage(Guid messageId, Stream stream, Metadata metadata, object data, long position, DateTime messageDate, string messageUser)
        {
            this.MessageId = messageId;
            this.Stream = stream;
            this.Metadata = metadata;
            this.Data = data;
            this.Position = position;
            this.MessageDate = messageDate;
            this.MessageUser = messageUser;

        }
        public Guid MessageId { get; private set; }
        public Stream Stream { get; private set; }
        public Metadata Metadata { get; private set; }
        public object Data { get; private set; }
        public long Position { get; private set; }
        public DateTime MessageDate { get; private set; }
        public string MessageUser { get; private set; }
    }
}