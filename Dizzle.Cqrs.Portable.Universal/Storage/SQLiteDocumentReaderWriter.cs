﻿//----------------------------------------------------------------------- 
// <copyright file="SQLiteDocumentReaderWriter.cs" company="4Deep Technologies LLC"> 
// Copyright (c) 4Deep Technologies LLC. All rights reserved. 
// <author>Darren Ford</author> 
// <date>Thursday, April 30, 2015 3:00:44 PM</date> 
// </copyright> 
//-----------------------------------------------------------------------


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dizzle.Cqrs.Portable.Storage.SQLite
{
    public sealed class SQLiteDocumentReaderWriter<TKey, TEntity> : IDocumentReader<TKey, TEntity>, IDocumentWriter<TKey, TEntity>
    {
        readonly IDocumentStrategy _strategy;
        readonly MessageStore _store;

        public SQLiteDocumentReaderWriter(IDocumentStrategy strategy, MessageStore store)
        {
            _store = store;
            _strategy = strategy;
        }

        string GetName(TKey key)
        {
            return _strategy.GetEntityLocation<TEntity>(key);
        }

        public bool TryGet(TKey key, out TEntity entity)
        {
            var name = GetName(key);
            byte[] bytes;
            if (_store.TryGetValue(name, out bytes))
            {
                using (var mem = new MemoryStream(bytes))
                {
                    entity = _strategy.Deserialize<TEntity>(mem);
                    return true;
                }
            }
            entity = default(TEntity);
            return false;
        }


        public TEntity AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update, AddOrUpdateHint hint)
        {
            var result = default(TEntity);
            _store.AddOrUpdate(GetName(key), s =>
            {
                result = addFactory();
                using (var memory = new MemoryStream())
                {
                    _strategy.Serialize(result, memory);
                    return memory.ToArray();
                }
            }, (s2, bytes) =>
            {
                TEntity entity;
                using (var memory = new MemoryStream(bytes))
                {
                    entity = _strategy.Deserialize<TEntity>(memory);
                }
                result = update(entity);
                using (var memory = new MemoryStream())
                {
                    _strategy.Serialize(result, memory);
                    return memory.ToArray();
                }
            });
            return result;
        }


        public bool TryDelete(TKey key)
        {
            return _store.TryRemove(GetName(key));
        }
    }

}
