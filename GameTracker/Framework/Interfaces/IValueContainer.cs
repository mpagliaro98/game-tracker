using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.LoadSave;

namespace RatableTracker.Framework.Interfaces
{
    public partial interface IValueContainer
    {
        void SetContent(string val);
        void SetContent(ISavable val);
        void SetContent(SavableRepresentation val);
        void SetContent(IEnumerable<string> val);
        void SetContent(IEnumerable<ISavable> val);
        void SetContent(IEnumerable<SavableRepresentation> val);
        string GetContentString();
        T GetContentISavable<T>() where T : ISavable, new();
        SavableRepresentation GetContentSavableRepresentation();
        IEnumerable<string> GetContentStringList();
        IEnumerable<T> GetContentISavableList<T>() where T : ISavable, new();
        IEnumerable<SavableRepresentation> GetContentSavableRepresentationList();
        bool HasValue();
        bool IsValueAList();
    }
}
