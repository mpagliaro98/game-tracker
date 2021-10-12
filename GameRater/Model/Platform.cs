using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker.Model
{
    public class Platform : ISavable
    {
        private string name = "";
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("name", name);
            return sr;
        }

        public void RestoreFromRepresentation(SavableRepresentation sr)
        {
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "name":
                        name = sr.GetValue(key);
                        break;
                    default:
                        Console.WriteLine("Platform.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }
    }
}
