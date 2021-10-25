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
            string content = FileLoadSaveInstance.ReadStringFromFile(filename);
            System.Diagnostics.Debug.WriteLine("Read " + Encoding.UTF8.GetByteCount(content).ToString() + " bytes from " + filename);
            return content;
        }
        
        public static void WriteStringToFile(string filename, string output)
        {
            FileLoadSaveInstance.WriteStringToFile(filename, output);
            System.Diagnostics.Debug.WriteLine("Wrote " + Encoding.UTF8.GetByteCount(output).ToString() + " bytes to " + filename);
        }
    }
}
