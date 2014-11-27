using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;

namespace Dizzle.Cqrs.Portable.Universal
{
    public static class Directory
    {
        public static bool Exists(string path)
        {
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder.GetFolderAsync(path).AsTask().Result;
                //no exception means file exists
                return true;
            }
            catch (Exception)
            {
                //find out through exception 
                return false;
            }
        }

        public static bool Delete(string path, bool recursive)
        {
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder.GetFolderAsync(path).AsTask().Result;
                folder.DeleteAsync().AsTask().Wait();
                //no exception means file exists
                return true;
            }
            catch (Exception )
            {
                //find out through exception 
                return false;
            }
        }

        public static bool CreateDirectory(string path)
        {
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder.CreateFolderAsync(path).AsTask().Result;
                //no exception means folder already exists
                return true;
            }
            catch (Exception )
            {
                //find out through exception 
                return false;
            }
        }
    }

    public static class File
    {
        public static bool Exists(string path)
        {
            try
            {
                var file = ApplicationData.Current.LocalFolder.GetFileAsync(path).AsTask().Result;
                //no exception means file exists
                return true;
            }
            catch (Exception )
            {
                //find out through exception 
                return false;
            }
        }

        public static bool Delete(string path)
        {
            try
            {
                var file = ApplicationData.Current.LocalFolder.GetFileAsync(path).AsTask().Result;
                file.DeleteAsync().AsTask().Wait();
                //no exception means file exists
                return true;
            }
            catch (Exception )
            {
                //find out through exception 
                return false;
            }
        }
    }
}
