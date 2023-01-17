using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Interfaces
{
    public interface IFileHandler
    {
        byte[] LoadFile(string path);
        void SaveFile(string path, byte[] data);
    }
}
