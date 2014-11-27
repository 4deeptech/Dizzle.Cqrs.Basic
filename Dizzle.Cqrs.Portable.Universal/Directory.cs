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
        public async static Task<bool> Exists(string path)
        {
            try
            {
                StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(path);
                //no exception means file exists
                return true;
            }
            catch (Exception)
            {
                //find out through exception 
                return false;
            }
        }

        public async static Task<bool> Delete(string path, bool recursive)
        {
            try
            {
                StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(path);
                await folder.DeleteAsync();
                //no exception means file exists
                return true;
            }
            catch (Exception )
            {
                //find out through exception 
                return false;
            }
        }

        public async static Task<bool> CreateDirectory(string path)
        {
            try
            {
                StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(path);
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
        public async static Task<bool> Exists(string path)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(path);
                //no exception means file exists
                return true;
            }
            catch (Exception )
            {
                //find out through exception 
                return false;
            }
        }

        public async static Task<bool> Delete(string path)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(path);
                await file.DeleteAsync();
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
