//----------------------------------------------------------------------- 
// <copyright file="IsolatedStorageDocumentReaderWriter.cs" company="4Deep Technologies LLC"> 
// Copyright (c) 4Deep Technologies LLC. All rights reserved. 
// <author>Darren Ford</author> 
// <date>Thursday, April 30, 2015 3:00:44 PM</date> 
// </copyright> 
//-----------------------------------------------------------------------


using Dizzle.Cqrs.Portable;
using Dizzle.Cqrs.Portable.Universal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Dizzle.Cqrs.Universal.Storage
{
    public sealed class IsolatedStorageDocumentReaderWriter<TKey, TEntity> : IDocumentReader<TKey, TEntity>,
                                                                             IDocumentWriter<TKey, TEntity>
    {
        readonly IDocumentStrategy _strategy;
        readonly string _folder;

        public IsolatedStorageDocumentReaderWriter(string directoryPath, IDocumentStrategy strategy)
        {
            _strategy = strategy;
            _folder = Path.Combine(directoryPath, strategy.GetEntityBucket<TEntity>());
        }

        public void InitIfNeeded()
        {
            Directory.CreateDirectory(_folder);
        }

        public bool TryGet(TKey key, out TEntity view)
        {
            view = default(TEntity);
            try
            {
                var name = GetName(key);
                if (!File.Exists(name))
                    return false;
                var file = ApplicationData.Current.LocalFolder.CreateFileAsync(name,CreationCollisionOption.OpenIfExists).AsTask().Result;
                using (var fileStream = file.OpenStreamForReadAsync().Result)
                {
                    using (var stream = new MemoryStream())
                    {
                        fileStream.CopyToAsync(stream).Wait();
                        stream.Seek(0, SeekOrigin.Begin);
                        if (stream.Length == 0)
                            return false;
                        view = _strategy.Deserialize<TEntity>(stream);
                        return true;
                    }
                }
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        private string GetName(TKey key)
        {
            return Path.Combine(_folder, _strategy.GetEntityLocation<TEntity>(key));
        }

        public TEntity AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update,
            AddOrUpdateHint hint)
        {
            var name = GetName(key);

            try
            {
                // This is fast and allows to have git-style subfolders in atomic strategy
                // to avoid NTFS performance degradation (when there are more than 
                // 10000 files per folder). Kudos to Gabriel Schenker for pointing this out
                var subfolder = Path.GetDirectoryName(name);
                if (subfolder != null && !Directory.Exists(subfolder))
                    Directory.CreateDirectory(subfolder);

                var file = ApplicationData.Current.LocalFolder.CreateFileAsync(name, CreationCollisionOption.OpenIfExists).AsTask().Result;

                byte[] initial = new byte[0];
                TEntity result;
                var props = file.GetBasicPropertiesAsync().AsTask().Result;
                if (props.Size == 0)
                {
                    result = addFactory();
                }
                else
                {
                    using (var mem = new MemoryStream())
                    {
                        using (var fileStream = file.OpenStreamForReadAsync().AsAsyncOperation().AsTask().Result)
                        {
                            fileStream.CopyToAsync(mem).Wait();
                            mem.Seek(0, SeekOrigin.Begin);
                            var entity = _strategy.Deserialize<TEntity>(mem);
                            initial = mem.ToArray();
                            result = update(entity);
                        }
                    }
                }

                // some serializers have nasty habit of closing the
                // underling stream
                using (var mem = new MemoryStream())
                {
                    _strategy.Serialize(result, mem);
                    var data = mem.ToArray();

                    if (!data.SequenceEqual(initial))
                    {
                        //overwrite file
                        file = ApplicationData.Current.LocalFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting).AsTask().Result;
                        FileIO.WriteBytesAsync(file, data).AsTask().Wait();
                    }
                }

                return result;

            }
            catch (Exception)
            {
                var s = string.Format(
                    "Container '{0}' does not exist.",
                    _folder);
                throw new InvalidOperationException(s);
            }
        }

        public bool TryDelete(TKey key)
        {
            var name = GetName(key);
            if (File.Exists(name))
            {
                File.Delete(name);
                return true;
            }
            return false;
        }
    }
}
