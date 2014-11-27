using Dizzle.Cqrs.Portable;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Dizzle.Cqrs.Portable.Universal;

namespace Dizzle.Cqrs.Universal.Storage
{
    public sealed class IsolatedStorageDocumentStore : IDocumentStore
    {
        readonly string _folderPath;
        readonly IDocumentStrategy _strategy;

        public IsolatedStorageDocumentStore(string folderPath, IDocumentStrategy strategy)
        {
            _folderPath = folderPath;
            _strategy = strategy;
        }

        public override string ToString()
        {
            return new Uri(_folderPath).AbsolutePath;
        }


        readonly HashSet<Tuple<Type, Type>> _initialized = new HashSet<Tuple<Type, Type>>();


        public IDocumentWriter<TKey, TEntity> GetWriter<TKey, TEntity>()
        {
            var container = new IsolatedStorageDocumentReaderWriter<TKey, TEntity>(_folderPath, _strategy);
            if (_initialized.Add(Tuple.Create(typeof(TKey), typeof(TEntity))))
            {
                container.InitIfNeeded();
            }
            return container;
        }

        public IDocumentReader<TKey, TEntity> GetReader<TKey, TEntity>()
        {
            return new IsolatedStorageDocumentReaderWriter<TKey, TEntity>(_folderPath, _strategy);
        }

        public IDocumentStrategy Strategy
        {
            get { return _strategy; }
        }


        public async Task<List<DocumentRecord>> EnumerateContents(string bucket)
        {
            List<DocumentRecord> contents = new List<DocumentRecord>();
            var full = Path.Combine(_folderPath, bucket);
            //Windows.Storage.ApplicationData.Current.LocalFolder.Path
            StorageFolder folder = null;

            try
            {
                folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(full);
                //no exception means file exists
            }
            catch (FileNotFoundException ex)
            {
                //find out through exception 
                return contents;
            }
            var fullFolder = folder.Path;
            foreach(StorageFile info in await folder.GetFilesAsync())
            {
                var path = fullFolder.Remove(0, fullFolder.Length + 1);
                IBuffer buf = await FileIO.ReadBufferAsync(info);
                using (DataReader dataReader = DataReader.FromBuffer(buf)) 
                {
                    byte[] bytes = new byte[buf.Length];
                    
                    contents.Add(new DocumentRecord(path, () => {dataReader.ReadBytes(bytes);return bytes;}));
                }
                
            }
            return contents;
            //var dir = new DirectoryInfo(full);
            //if (!dir.Exists) yield break;

            //var fullFolder = dir.FullName;
            //foreach (var info in dir.EnumerateFiles("*", SearchOption.AllDirectories))
            //{
            //    var fullName = info.FullName;
            //    var path = fullName.Remove(0, fullFolder.Length + 1).Replace(Path.DirectorySeparatorChar, '/');
            //    yield return new DocumentRecord(path, () => File.ReadAllBytes(fullName));
            //}
        }

        public async void WriteContents(string bucket, IEnumerable<DocumentRecord> records)
        {
            var buck = Path.Combine(_folderPath, bucket);
            StorageFolder folder = null;
            bool created = false;
            try
            {
                folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(buck);
                //no exception means folder exists
                created = true;
            }
            catch (FileNotFoundException ex) { }
            if (!created)
            {
                //create it
                folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(buck);
            }
            foreach (var pair in records)
            {
                var recordPath = Path.Combine(buck, pair.Key);
                //bool createdRecord = false;
                //try
                //{
                //    folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(recordPath);
                //    //no exception means folder exists
                //    createdRecord = true;
                //}
                //catch (FileNotFoundException ex) { }
                if (!await File.Exists(recordPath))
                {
                    //create it
                    folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(recordPath);
                }
                //write bytes to file
                StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(recordPath, CreationCollisionOption.ReplaceExisting);
                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    byte[] fileBytes = pair.Read();
                    stream.Write(fileBytes, 0, fileBytes.Length);
                }
            }
            //foreach (var pair in records)
            //{
            //    var recordPath = Path.Combine(buck, pair.Key);

            //    var path = Path.GetDirectoryName(recordPath) ?? "";
            //    if (!Directory.Exists(path))
            //    {
            //        Directory.CreateDirectory(path);
            //    }
            //    File.WriteAllBytes(recordPath, pair.Read());
            //}
        }

        public async void ResetAll()
        {
            if (await Directory.Exists(_folderPath))
                await Directory.Delete(_folderPath, true);
            await Directory.CreateDirectory(_folderPath);
        }

        public async void Reset(string bucket)
        {
            var path = Path.Combine(_folderPath, bucket);
            if (await Directory.Exists(path))
                await Directory.Delete(path, true);
            await Directory.CreateDirectory(path);
        }
    }
}
