﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dizzle.Cqrs.Portable
{
    public enum AddOrUpdateHint
    {
        ProbablyExists,
        ProbablyDoesNotExist
    }


    /// <summary>
    /// View writer interface, used by the event handlers
    /// </summary>
    /// <typeparam name="TEntity">The type of the view.</typeparam>
    /// <typeparam name="TKey">type of the key</typeparam>
    public interface IDocumentWriter<in TKey, TEntity>
    {
        Task<TEntity> AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists);
        Task<bool> TryDelete(TKey key);
    }
}
