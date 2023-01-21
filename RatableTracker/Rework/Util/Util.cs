using RatableTracker.Rework.Interfaces;
using RatableTracker.Rework.Model;
using RatableTracker.Rework.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Rework.Util
{
    public static class Util
    {
        public static Encoding TextEncoding => Encoding.UTF8;

        public static VersionNumber FrameworkVersion => new VersionNumber(2, 0, 0, 0);

        public static T FindObjectInList<T>(IList<T> list, UniqueID uniqueID) where T : IKeyable
        {
            foreach (T item in list)
            {
                if (item.UniqueID.Equals(uniqueID)) return item;
            }
            return default; // null
        }

        public static IEnumerable<string> ConvertListToStringList<T>(IEnumerable<T> vals)
        {
            LinkedList<string> list = new LinkedList<string>();
            foreach (T value in vals)
            {
                list.AddLast(value.ToString());
            }
            return list;
        }

        public static void SaveOne<T>(TrackerModule module, T obj, ILoadSaveMethod conn, Action<T> saveAction) where T : SavableObject
        {
            try
            {
                obj.Validate(module.Logger);
                saveAction(obj);
                obj.PostSave(module);
            }
            catch
            {
                conn.SetCancel(true);
                throw;
            }
        }

        public static void DeleteOne<T>(TrackerModule module, T obj, ILoadSaveMethod conn, Action<T> deleteAction) where T : SaveDeleteObject
        {
            try
            {
                deleteAction(obj);
                obj.PostDelete(module);
            }
            catch
            {
                conn.SetCancel(true);
                throw;
            }
        }
    }
}
