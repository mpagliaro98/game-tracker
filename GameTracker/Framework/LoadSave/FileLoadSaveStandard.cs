using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.LoadSave
{
    public class FileLoadSaveStandard : IFileLoadSave
    {
        protected const string DIRECTORY_SAVE = "savefiles";
        protected static string SAVE_DIR = PathController.PathController.Combine(PathController.PathController.BaseDirectory(), DIRECTORY_SAVE);

        public string ReadStringFromFile(string filename)
        {
            string filepath = PathController.PathController.Combine(SAVE_DIR, filename);
            PathController.PathController.CreateFileIfDoesNotExist(filepath);
            string output = PathController.PathController.ReadFromFile(filepath);
            System.Diagnostics.Debug.WriteLine("Read " + Encoding.UTF8.GetByteCount(output).ToString() + " bytes from " + filepath);
            return output;
        }

        public void WriteStringToFile(string filename, string output)
        {
            string filepath = PathController.PathController.Combine(SAVE_DIR, filename);
            PathController.PathController.CreateFileIfDoesNotExist(filepath);
            PathController.PathController.WriteToFile(filepath, output);
            System.Diagnostics.Debug.WriteLine("Wrote " + Encoding.UTF8.GetByteCount(output).ToString() + " bytes to " + filepath);
        }
    }
}
