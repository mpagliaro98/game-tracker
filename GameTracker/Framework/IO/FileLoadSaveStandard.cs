using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.IO
{
    public class FileLoadSaveStandard : IFileLoadSave
    {
        protected const string DIRECTORY_SAVE = "savefiles";
        protected static string SAVE_DIR = IO.PathController.Combine(IO.PathController.BaseDirectory(), DIRECTORY_SAVE);

        public string ReadStringFromFile(string filename)
        {
            string filepath = PathController.Combine(SAVE_DIR, filename);
            PathController.CreateFileIfDoesNotExist(filepath);
            return PathController.ReadFromFile(filepath);
        }

        public async Task<string> ReadStringFromFileAsync(string filename)
        {
            string filepath = PathController.Combine(SAVE_DIR, filename);
            PathController.CreateFileIfDoesNotExist(filepath);
            return await PathController.ReadFromFileAsync(filepath);
        }

        public void WriteStringToFile(string filename, string output)
        {
            string filepath = PathController.Combine(SAVE_DIR, filename);
            PathController.CreateFileIfDoesNotExist(filepath);
            PathController.WriteToFile(filepath, output);
        }

        public async Task WriteStringToFileAsync(string filename, string output)
        {
            string filepath = PathController.Combine(SAVE_DIR, filename);
            PathController.CreateFileIfDoesNotExist(filepath);
            await PathController.WriteToFileAsync(filepath, output);
        }
    }
}
