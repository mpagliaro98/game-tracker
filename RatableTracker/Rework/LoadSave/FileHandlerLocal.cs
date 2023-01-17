using RatableTracker.Rework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.LoadSave
{
    public class FileHandlerLocal : IFileHandler
    {
        public FileHandlerLocal(string directory, IPathController pathController)
        {

        }

        public byte[] LoadFile(string path)
        {
            throw new NotImplementedException();
        }

        public void SaveFile(string path, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
