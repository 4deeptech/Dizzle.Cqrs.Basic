//----------------------------------------------------------------------- 
// <copyright file="SQLiteEventStore.cs" company="4Deep Technologies LLC"> 
// Copyright (c) 4Deep Technologies LLC. All rights reserved. 
// <author>Darren Ford</author> 
// <date>Thursday, April 30, 2015 3:00:44 PM</date> 
// </copyright> 
//-----------------------------------------------------------------------


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
        public string UniqueId
        {
            get
            {
                return Id + "-" + Version;
            }

            set
            {
                //ignore
            }
        }
        [Indexed]
        public string Id { get; set; }
        public byte[] Data { get; set; }
        [Indexed]
        public long StoreVersion { get; set; }
        [Indexed]
        public long Version { get; set; }
        public DateTimeOffset DateLastModified { get; set; }

        public EventStream()
        {

        }

        public EventStream(string id, byte[] data, long version, DateTimeOffset lastModified)
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
        long _storeVersion = 0L;
        public MessageStore(string path, ISQLitePlatform platform)
        {
            _storeVersion = 0L;
            _path = path;
            _db = new SQLiteConnection(platform, _path);
            _db.CreateTable<EventStream>();
            //read max store version
            var command = _db.CreateCommand("SELECT Coalesce(MAX(StoreVersion),1) FROM Streams");
            var result = command.ExecuteScalar<long>();
            _storeVersion = result;
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
                stream.StoreVersion = _storeVersion; 
                stream.Version = frame.Version;
                stream.DateLastModified = DateTimeOffset.UtcNow;
                stream.Id = id;
                _db.Insert(stream);
                _storeVersion++;
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
