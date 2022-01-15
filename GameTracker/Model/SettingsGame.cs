using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework;
using RatableTracker.Framework.LoadSave;

namespace GameTracker.Model
{
    public class SettingsGame : SettingsScore
    {
        public string AWSKeyFilePath { get; set; } = "";

        public SettingsGame() : base() { }

        public override SavableRepresentation<T> LoadIntoRepresentation<T>()
        {
            SavableRepresentation<T> sr = base.LoadIntoRepresentation<T>();
            sr.SaveValue("awsKeyFilePath", AWSKeyFilePath);
            return sr;
        }

        public override void RestoreFromRepresentation<T>(SavableRepresentation<T> sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "awsKeyFilePath":
                        AWSKeyFilePath = sr.GetString(key);
                        break;
                    default:
                        break;
                }
            }
        }

        public bool IsUsingAWS()
        {
            return AWSKeyFilePath != "";
        }
    }
}
