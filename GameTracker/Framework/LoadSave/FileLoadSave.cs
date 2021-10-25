using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.LoadSave
{
    public static class FileLoadSave
    {
        public static IFileLoadSave FileLoadSaveInstance { get; set; } = new FileLoadSaveStandard();

        public static string ReadStringFromFile(string filename)
        {
            return FileLoadSaveInstance.ReadStringFromFile(filename);
        }
        
        public static void WriteStringToFile(string filename, string output)
        {
            FileLoadSaveInstance.WriteStringToFile(filename, output);
        }
    }
}
