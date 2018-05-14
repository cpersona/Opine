using System;

namespace Opine.Messaging
{
    public class Metadata
    {
        public Metadata(object aggregateId)
        {
            AggregateId = aggregateId;
        }

        public Metadata(object aggregateId, string processCode, object processId, DateTime messageDate)
        {
            AggregateId = aggregateId;
            ProcessCode = processCode;
            ProcessId = processId;
            MessageDate = messageDate;
        }

        public object AggregateId { get; private set; }
        public string ProcessCode { get; private set; }
        public object ProcessId { get; private set; }
        public DateTime MessageDate { get; private set; }
    }
}