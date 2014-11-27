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

        public async void InitIfNeeded()
        {
            await Directory.CreateDirectory(_folder);
        }

        public bool TryGet(TKey key, out TEntity view)
        {
            view = default(TEntity);
            try
            {
                var name = GetName(key);
                if (!File.Exists(name).Result)
                    return false;
                var file = ApplicationData.Current.LocalFolder.CreateFileAsync(name,CreationCollisionOption.OpenIfExists).AsTask().Result;
                using (var fileStream = file.OpenStreamForReadAsync().Result)
                {
                    using (var stream = new MemoryStream())
                    {
                        fileStream.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        if (stream.Length == 0)
                            return false;
                        view = _strategy.Deserialize<TEntity>(stream);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        string GetName(TKey key)
        {
            return Path.Combine(_folder, _strategy.GetEntityLocation<TEntity>(key));
        }

        public async Task<TEntity> AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update,
            AddOrUpdateHint hint)
        {
            var name = GetName(key);

            try
            {
                // This is fast and allows to have git-style subfolders in atomic strategy
                // to avoid NTFS performance degradation (when there are more than 
                // 10000 files per folder). Kudos to Gabriel Schenker for pointing this out
                var subfolder = Path.GetDirectoryName(name);
                if (subfolder != null && !await Directory.Exists(subfolder))
                    await Directory.CreateDirectory(subfolder);


                // we are locking this file.
                //using (var file = File.Open(name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                //using (
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(name,CreationCollisionOption.OpenIfExists);
                
                //{
                    byte[] initial = new byte[0];
                    TEntity result;
                    var props = await file.GetBasicPropertiesAsync();
                    if (props.Size == 0)
                    {
                        result = addFactory();
                    }
                    else
                    {
                        using (var mem = new MemoryStream())
                        {
                            using (var fileStream = await file.OpenStreamForReadAsync())
                            {
                                await fileStream.CopyToAsync(mem);
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
                            //await file.DeleteAsync();
                            file = await ApplicationData.Current.LocalFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
                            await FileIO.WriteBytesAsync(file, data);
                        }
                    }

                    return result;
                //}
            }
            catch (Exception)
            {
                var s = string.Format(
                    "Container '{0}' does not exist.",
                    _folder);
                throw new InvalidOperationException(s);
            }
        }

        public async Task<bool> TryDelete(TKey key)
        {
            var name = GetName(key);
            if (await File.Exists(name))
            {
                await File.Delete(name);
                return true;
            }
            return false;
        }
    }
}
