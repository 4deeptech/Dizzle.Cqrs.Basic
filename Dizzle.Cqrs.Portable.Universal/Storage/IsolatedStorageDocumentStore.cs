//----------------------------------------------------------------------- 
// <copyright file="IsolatedStorageDocumentStore.cs" company="4Deep Technologies LLC"> 
// Copyright (c) 4Deep Technologies LLC. All rights reserved. 
// <author>Darren Ford</author> 
// <date>Thursday, April 30, 2015 3:00:44 PM</date> 
// </copyright> 
//-----------------------------------------------------------------------


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


        public List<DocumentRecord> EnumerateContents(string bucket)
        {
            List<DocumentRecord> contents = new List<DocumentRecord>();
            var full = Path.Combine(_folderPath, bucket);
            StorageFolder folder = null;

            try
            {
                folder = ApplicationData.Current.LocalFolder.GetFolderAsync(full).AsTask().Result;
                //no exception means file exists
            }
            catch (FileNotFoundException ex)
            {
                //find out through exception 
                return contents;
            }
            var fullFolder = folder.Path;
            foreach (StorageFile info in folder.GetFilesAsync().AsTask().Result)
            {
                var path = fullFolder.Remove(0, fullFolder.Length + 1);
                IBuffer buf = FileIO.ReadBufferAsync(info).AsTask().Result;
                using (DataReader dataReader = DataReader.FromBuffer(buf)) 
                {
                    byte[] bytes = new byte[buf.Length];
                    
                    contents.Add(new DocumentRecord(path, () => {dataReader.ReadBytes(bytes);return bytes;}));
                }
                
            }
            return contents;
        }

        public void WriteContents(string bucket, IEnumerable<DocumentRecord> records)
        {
            var buck = Path.Combine(_folderPath, bucket);
            StorageFolder folder = null;
            bool created = false;
            try
            {
                folder = ApplicationData.Current.LocalFolder.GetFolderAsync(buck).AsTask().Result;
                //no exception means folder exists
                created = true;
            }
            catch (FileNotFoundException ex) { }
            if (!created)
            {
                //create it
                folder = ApplicationData.Current.LocalFolder.CreateFolderAsync(buck).AsTask().Result;
            }
            foreach (var pair in records)
            {
                var recordPath = Path.Combine(buck, pair.Key);
                
                if (!File.Exists(recordPath))
                {
                    //create it
                    folder = ApplicationData.Current.LocalFolder.CreateFolderAsync(recordPath).AsTask().Result;
                }
                //write bytes to file
                StorageFile file = Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(recordPath, CreationCollisionOption.ReplaceExisting).AsTask().Result;
                using (var stream = file.OpenStreamForWriteAsync().AsAsyncOperation().AsTask().Result)
                {
                    byte[] fileBytes = pair.Read();
                    stream.Write(fileBytes, 0, fileBytes.Length);
                }
            }
        }

        public void ResetAll()
        {
            if (Directory.Exists(_folderPath))
                Directory.Delete(_folderPath, true);
            Directory.CreateDirectory(_folderPath);
        }

        public void Reset(string bucket)
        {
            var path = Path.Combine(_folderPath, bucket);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }
    }
}
