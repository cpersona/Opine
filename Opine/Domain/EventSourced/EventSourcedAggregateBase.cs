using System;
using System.Collections.Generic;

namespace Opine.Domain.EventSourced
{
    public abstract class EventSourcedAggregateBase<TRoot> : Aggregate<TRoot>
    {
        public EventSourcedAggregateBase(TRoot root, long version, IEnumerable<IEvent> events) : base(root, version)
        {
            ApplyAll(events);
        }

        protected override void Emit(IEvent e)
        {
            // Modify the root using this method
            Apply(e);
            base.Emit(e);
        }

        protected void ApplyAll(IEnumerable<IEvent> events)
        {
            foreach (var e in events)
            {
                Apply(e);
            }
        }

        protected abstract void Apply(IEvent e);
    }
}