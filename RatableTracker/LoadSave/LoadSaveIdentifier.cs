using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatableTracker.Framework.LoadSave
{
    public class LoadSaveIdentifier
    {
        private readonly string id = "";
        public string ID => id;

        public LoadSaveIdentifier(string id)
        {
            this.id = id;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                LoadSaveIdentifier o = (LoadSaveIdentifier)obj;
                return ID == o.ID;
            }
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return ID;
        }
    }
}
