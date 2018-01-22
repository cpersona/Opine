using System;
using System.Collections.Generic;

namespace Opine.Domain.EventSourced
{
    public class EventSourcedAggregate<TRoot> : EventSourcedAggregateBase<TRoot>
    {
        private Dictionary<Type, Action<IEvent>> map = 
            new Dictionary<Type, Action<IEvent>>();

        public EventSourcedAggregate(TRoot root, long version) : base(root, version, Array.Empty<IEvent>())
        {
            
        }

        protected void When<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            map.Add(typeof(TEvent), e => handler((TEvent)e));
        }

        protected override void Apply(IEvent e)
        {
            Action<IEvent> handler = null;
            if (map.TryGetValue(e.GetType(), out handler))
            {
                handler(e);
            }
        }
    }
}