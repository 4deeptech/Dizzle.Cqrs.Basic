using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using SQLite.Net;
using SQLite.Net.Attributes;
using SQLite.Net.Interop;
using System.IO;
using Newtonsoft.Json;

namespace Dizzle.Cqrs.Portable.Storage.SQLite.Events
{
    public class SQLiteEventStore : IEventStore
    {
        readonly MessageStore _store;

        public SQLiteEventStore(string path, ISQLitePlatform platform)
        {
            _store = new MessageStore(path, platform);
        }

        public IEnumerable<IEvent> LoadEventsFor<TAggregate>(string id)
        {
            return _store.LoadEventsForStream(id);
        }

        public IEnumerable<IEvent> LoadEventsFor<TAggregate>(string id, long afterVersion)
        {
            return _store.LoadEventsForStream(id, afterVersion);
        }

        public void SaveEventsFor<TAggregate>(string aggregateId, long eventsLoaded, IEnumerable<IEvent> newEvents)
        {
            _store.SaveEventsForStream(aggregateId, eventsLoaded, newEvents);
        }

        public void Clear()
        {
            _store.Clear();
        }

        public void Close()
        {
            _store.Close();
        }

        private AbstractIdentity<Guid> GetAggregateIdFromEvent(object e)
        {
            var idField = e.GetType().GetTypeInfo().DeclaredFields.Single(t=>t.Name.Equals("Id"));
            if (idField == null)
                throw new Exception("Event type " + e.GetType().Name + " is missing an Id field");
            return (AbstractIdentity<Guid>)idField.GetValue(e);
        }
    }

    #region SQLite Storage Class

    [Table("Streams")]
    public class EventStream
    {
        [PrimaryKey]
        public string Id { get; set; }
        public byte[] Data { get; set; }
        public long EventVersion { get; set; }
        public long Version { get; set; }
        public DateTime DateLastModified { get; set; }

        public EventStream()
        {

        }

        public EventStream(string id, byte[] data, long version, DateTime lastModified)
        {
            Id = id;
            Data = data;
            Version = version;
            DateLastModified = lastModified;
        }
    }

    #endregion

    #region Message Store Abstraction

    public class MessageStore : IDisposable
    {
        readonly string _path = null;
        SQLiteConnection _db = null;
        public MessageStore(string path, ISQLitePlatform platform)
        {
            _path = path;
            _db = new SQLiteConnection(platform, _path);
            _db.CreateTable<EventStream>();
        }

        public List<IEvent> LoadEventsForStream(string key)
        {
            return LoadEventsForStream(key,0);
        }

        public List<IEvent> LoadEventsForStream(string key, long afterVersion)
        {
            List<IEvent> eventList = new List<IEvent>();
            try
            {
                EventStorageStrategy strategy = new EventStorageStrategy();
                List<EventStream> events = _db.Query<EventStream>("select * from Streams where Id = ? and Version > ?", key, afterVersion);
                
                foreach (EventStream es in events)
                {
                    using (var memory = new MemoryStream())
                    {
                        memory.Write(es.Data, 0, es.Data.Length);
                        //reset
                        memory.Position = 0;
                        eventList.AddRange(strategy.Deserialize<StorageFrame>(memory).Events);
                    }
                }
                return eventList;
            }
            catch
            {
                return eventList;
            }
        }

        public void SaveEventsForStream(string id, long version, IEnumerable<IEvent> newEvents)
        {
            try
            {
                StorageFrame frame = new StorageFrame(newEvents, version + 1);
                EventStorageStrategy strategy = new EventStorageStrategy();
                EventStream stream = new EventStream();
                using (var memory = new MemoryStream())
                {
                    strategy.Serialize(frame, memory);
                    stream.Data = memory.ToArray();
                }

                stream.Version = frame.Version;
                stream.DateLastModified = DateTime.UtcNow;
                stream.Id = id;
                _db.Insert(stream);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void Clear()
        {
            try
            {
                _db.DeleteAll<EventStream>();
            }
            catch (SQLiteException ex)
            {
            }
        }

        public void Close()
        {
            try
            {
                _db.Close();
            }
            catch (SQLiteException ex)
            {
            }
        }

        public void Dispose()
        {
            Close();
        }
    }

    #endregion
}
