using SQLite.Net;
using SQLite.Net.Attributes;
using SQLite.Net.Interop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dizzle.Cqrs.Portable.Storage.SQLite
{
    public sealed class SQLiteDocumentStore : IDocumentStore
    {
        readonly IDocumentStrategy _strategy;
        readonly MessageStore _store;
        

        public SQLiteDocumentStore(string path,ISQLitePlatform platform,IDocumentStrategy strategy)
        {
            _store = new MessageStore(path, platform);
            _strategy = strategy;
        }

        public IDocumentWriter<TKey, TEntity> GetWriter<TKey, TEntity>()
        {
            var bucket = _strategy.GetEntityBucket<TEntity>();
            //var store = _store.GetOrAdd(bucket, s => new ConcurrentDictionary<string, byte[]>());
            return new SQLiteDocumentReaderWriter<TKey, TEntity>(_strategy, _store);
        }


        public void WriteContents(string bucket, IEnumerable<DocumentRecord> records)
        {
            var pairs = records.Select(r => new KeyValuePair<string, byte[]>(r.Key, r.Read())).ToArray();
            //_store[bucket] = new ConcurrentDictionary<string, byte[]>(pairs);
        }

        public void ResetAll()
        {
            _store.Clear();
        }

        public void Reset(string bucketNames)
        {
            _store.TryRemove(bucketNames);
        }


        public IDocumentReader<TKey, TEntity> GetReader<TKey, TEntity>()
        {
            var bucket = _strategy.GetEntityBucket<TEntity>();
            //var store = _store.GetOrAdd(bucket, s => new ConcurrentDictionary<string, byte[]>());
            return new SQLiteDocumentReaderWriter<TKey, TEntity>(_strategy, _store);
        }

        public IDocumentStrategy Strategy
        {
            get { return _strategy; }
        }

        public List<DocumentRecord> EnumerateContents(string bucket)
        {
            //var store = _store.GetOrAdd(bucket, s => new ConcurrentDictionary<string, byte[]>());
            byte[] buffer;
            _store.TryGetValue(bucket, out buffer);
            List<DocumentRecord> records = new List<DocumentRecord>();
            records.Add(new DocumentRecord(bucket, () => buffer));
            return records;
        }
    }

    [Table("Documents")]
    public class Document
    {
        [PrimaryKey]
        public string Id { get; set; }
        public byte[] Data { get; set; }
        public string Bucket { get; set; }
        public DateTime DateLastModified { get; set; }
    }

    public class MessageStore
    {
        readonly string _path = null;
        SQLiteConnection _db = null;
        public MessageStore(string path, ISQLitePlatform platform)
        {
            _path = path;
            _db = new SQLiteConnection(platform, _path);
            _db.CreateTable<Document>();
        }

        public bool TryGetValue(string key,out byte[] bytes)
        {
            try
            {
                Document doc = _db.Get<Document>(key);
                if (doc != null)
                {
                    bytes = doc.Data;
                    return true;
                }
                bytes = null;
                return false;
            }
            catch
            {
                bytes = null;
                return false;
            }
        }

        public byte[] AddOrUpdate(string key, Func<string, byte[]> addValueFactory, Func<string, byte[], byte[]> updateValueFactory)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (addValueFactory == null) throw new ArgumentNullException("addValueFactory");
            if (updateValueFactory == null) throw new ArgumentNullException("updateValueFactory");
            byte[] newValue;
            while (true)
            {
                byte[] oldValue;
                if (TryGetValue(key, out oldValue))
                //key exists, try to update
                {
                    newValue = updateValueFactory(key, oldValue);
                    Document doc = _db.Get<Document>(key);
                    if (doc != null)
                    {
                        doc.Data = newValue;
                        _db.Update(doc);
                        return newValue;
                    }
                }
                else //try add
                {
                    newValue = addValueFactory(key);
                    Document doc = new Document();
                    doc.Id = key;
                    doc.Bucket = key;
                    doc.Data = newValue;
                    _db.Insert(doc);
                    return newValue;
                }
            }
        }

        public bool TryRemove(string key)
        {
            int rowcount = _db.Delete<Document>(key);
            if (rowcount > 0)
                return true;
            else return false;
        }

        public void Clear()
        {
            try
            {
                _db.DeleteAll<Document>();
            }
            catch(SQLiteException ex)
            {
                //no such table if it has never been run
            }
        }
    }

}
