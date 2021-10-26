using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.Interfaces
{
    public interface IContentLoadSave<TRead, TWrite>
    {
        TRead Read(string key);
        Task<TRead> ReadAsync(string key);
        void Write(string key, TWrite output);
        Task WriteAsync(string key, TWrite output);
    }
}
