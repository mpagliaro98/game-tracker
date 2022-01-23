using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.Interfaces;

namespace RatableTracker.Framework.IO
{
    public abstract class ContentLoadSaveTransferBase<T1Read, T1Write, T2Read, T2Write>
    {
        public abstract void TransferSaveFiles(IContentLoadSave<T1Read, T1Write> from, IContentLoadSave<T2Read, T2Write> to, IEnumerable<string> keys);
        public abstract Task TransferSaveFilesAsync(IContentLoadSave<T1Read, T1Write> from, IContentLoadSave<T2Read, T2Write> to, IEnumerable<string> keys);
    }
}
