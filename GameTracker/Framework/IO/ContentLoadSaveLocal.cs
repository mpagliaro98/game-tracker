using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.IO
{
    public class ContentLoadSaveLocal : IContentLoadSave<string, string>
    {
        protected const string DIRECTORY_SAVE = "savefiles";
        protected static string SAVE_DIR = IO.PathController.Combine(IO.PathController.BaseDirectory(), DIRECTORY_SAVE);

        public string Read(string key)
        {
            string filepath = PathController.Combine(SAVE_DIR, key);
            PathController.CreateFileIfDoesNotExist(filepath);
            return PathController.ReadFromFile(filepath);
        }

        public async Task<string> ReadAsync(string key)
        {
            string filepath = PathController.Combine(SAVE_DIR, key);
            PathController.CreateFileIfDoesNotExist(filepath);
            return await PathController.ReadFromFileAsync(filepath);
        }

        public void Write(string key, string output)
        {
            string filepath = PathController.Combine(SAVE_DIR, key);
            PathController.CreateFileIfDoesNotExist(filepath);
            PathController.WriteToFile(filepath, output);
        }

        public async Task WriteAsync(string key, string output)
        {
            string filepath = PathController.Combine(SAVE_DIR, key);
            PathController.CreateFileIfDoesNotExist(filepath);
            await PathController.WriteToFileAsync(filepath, output);
        }
    }
}
