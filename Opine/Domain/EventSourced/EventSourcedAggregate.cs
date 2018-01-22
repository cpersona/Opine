using System;
using System.Collections.Generic;

namespace Opine.Domain.EventSourced
{
    public class EventSourcedAggregate<TRoot> : Aggregate<TRoot>
    {
        private Dictionary<Type, Action<IEvent>> map = 
            new Dictionary<Type, Action<IEvent>>();

        public EventSourcedAggregate(TRoot root, long version) : base(root, version)
        {
            
        }

        protected override void Emit(IEvent e)
        {
            // Modify the root using this method
            Apply(e);
            base.Emit(e);
        }

        protected void When<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            map.Add(typeof(TEvent), e => handler((TEvent)e));
        }

        protected void ApplyAll(IEnumerable<IEvent> events)
        {
            foreach (var e in events)
            {
                Apply(e);
            }
        }

        protected void Apply(IEvent e)
        {
            Action<IEvent> handler = null;
            if (map.TryGetValue(e.GetType(), out handler))
            {
                handler(e);
            }
        }
    }
}