using System.Collections.Generic;

namespace Opine.Domain
{
    public class Aggregate<TRoot> : IAggregate<TRoot>
    {
        private List<IEvent> events = new List<IEvent>();
        private TRoot root;
        private long version;

        public Aggregate(TRoot root, long version)
        {
            this.root = root;
            this.version = version;
        }

        public TRoot Root 
        {
            get => root;
            protected set => root = value;
        }

        public IEnumerable<IEvent> Events => events;

        public object RootObject => root;

        public long Version => version;

        protected virtual void Emit(IEvent e)
        {
            events.Add(e);
        }
    }
}