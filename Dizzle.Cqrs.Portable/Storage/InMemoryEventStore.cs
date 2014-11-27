﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

namespace Dizzle.Cqrs.Portable
{
    public class InMemoryEventStore : IEventStore
    {
        private class Stream
        {
            public List<IEvent> Events;
            public long Version { get; set; }
        }

        private ConcurrentDictionary<string, Stream> store =
            new ConcurrentDictionary<string, Stream>();

        public IEnumerable<IEvent> LoadEventsFor<TAggregate>(string id)
        {
            // Get the current event stream; note that we never mutate the
            // Events array so it's safe to return the real thing.
            Stream s;
            if (store.TryGetValue(id, out s))
                return s.Events;
            else
                return new List<IEvent>();
        }

        public void SaveEventsFor<TAggregate>(string aggregateId, int eventsLoaded, IEnumerable<IEvent> newEvents)
        {   
            // Get or create stream.
            var s = store.GetOrAdd(aggregateId, _ => new Stream());

            // We'll use a lock-free algorithm for the update.
            while (true)
            {
                // Read the current event list.
                var eventList = s.Events;

                // Ensure no events persisted since us.
                var prevEvents = eventList == null ? 0 : eventList.Count;
                if (prevEvents != eventsLoaded)
                    throw new Exception("Concurrency conflict; cannot persist these events");

                // Create a new event list with existing ones plus our new
                // ones (making new important for lock free algorithm!)
                var newEventList = eventList == null
                    ? new List<IEvent>()
                    : (List<IEvent>)eventList.Clone();
                newEventList.AddRange(newEvents);

                // Try to put the new event list in place atomically.
                if (Interlocked.CompareExchange(ref s.Events, newEventList, eventList) == eventList)
                    break;
            }
        }

        private AbstractIdentity<Guid> GetAggregateIdFromEvent(object e)
        {
            var idField = e.GetType().GetTypeInfo().DeclaredFields.Single(t=>t.Name.Equals("Id"));
            if (idField == null)
                throw new Exception("Event type " + e.GetType().Name + " is missing an Id field");
            return (AbstractIdentity<Guid>)idField.GetValue(e);
        }

        
    }

    public static class Extensions
    {
        public static List<IEvent> Clone(this List<IEvent> currentList)
        {
            List<IEvent> newList = new List<IEvent>();
            newList.AddRange(currentList);
            return newList;
        }
    }
}
