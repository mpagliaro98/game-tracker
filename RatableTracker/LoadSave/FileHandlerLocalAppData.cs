using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.LoadSave
{
    public class FileHandlerLocalAppData : FileHandlerLocal
    {
        public FileHandlerLocalAppData(IPathController pathController) : base(pathController.ApplicationDirectory(), pathController) { }

        public FileHandlerLocalAppData(IPathController pathController, string subDirectory) : base(pathController.Combine(pathController.ApplicationDirectory(), subDirectory), pathController) { }
    }
}
