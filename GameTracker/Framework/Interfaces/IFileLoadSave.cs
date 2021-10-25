using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Interfaces
{
    public interface IFileLoadSave
    {
        string ReadStringFromFile(string filename);
        Task<string> ReadStringFromFileAsync(string filename);
        void WriteStringToFile(string filename, string output);
        Task WriteStringToFileAsync(string filename, string output);
    }
}
