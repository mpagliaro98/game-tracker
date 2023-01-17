using RatableTracker.Rework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.LoadSave
{
    public class FileHandlerLocalAppData : FileHandlerLocal
    {
        // TODO
        public FileHandlerLocalAppData(IPathController pathController) : base("local app data", pathController) { }
    }
}
