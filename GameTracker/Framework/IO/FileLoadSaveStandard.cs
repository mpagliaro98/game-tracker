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
            string filepath = IO.PathController.Combine(SAVE_DIR, filename);
            IO.PathController.CreateFileIfDoesNotExist(filepath);
            return IO.PathController.ReadFromFile(filepath);
        }

        public void WriteStringToFile(string filename, string output)
        {
            string filepath = IO.PathController.Combine(SAVE_DIR, filename);
            IO.PathController.CreateFileIfDoesNotExist(filepath);
            IO.PathController.WriteToFile(filepath, output);
        }
    }
}
