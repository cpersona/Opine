using System;

namespace Opine.Job
{
    public class EventStoreOptions
    {
        public int BufferSize { get; set; } = 100;
        public string EventStoreUri { get; set; }
    }
}