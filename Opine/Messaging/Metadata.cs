namespace Opine.Messaging
{
    public class Metadata
    {
        public Metadata(object aggregateId)
        {
            AggregateId = aggregateId;
        }

        public Metadata(object aggregateId, string processCode, object processId)
        {
            AggregateId = aggregateId;
            ProcessCode = processCode;
            ProcessId = processId;
        }

        public object AggregateId { get; private set; }
        public string ProcessCode { get; private set; }
        public object ProcessId { get; private set; }
    }
}