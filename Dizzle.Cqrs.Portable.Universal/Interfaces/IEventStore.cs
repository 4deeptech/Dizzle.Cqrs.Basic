using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Dizzle.Cqrs.Portable
{
    public interface IEventStore
    {
        IEnumerable<IEvent> LoadEventsFor<TAggregate>(string streamName);
        void SaveEventsFor<TAggregate>(string streamName, int eventsLoaded, IEnumerable<IEvent> newEvents);
    }
}
