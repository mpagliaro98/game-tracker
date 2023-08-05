using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Interfaces
{
    public interface ISaveBackupHandler
    {
        byte[] ExportSaveBackup();
        void ImportSaveBackup(byte[] contents);
    }
}
