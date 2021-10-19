using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework;

namespace GameTracker.Model
{
    public class Platform : ISavable, IReferable, IModuleAccess
    {
        private string name = "";
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Guid referenceKey = Guid.NewGuid();
        public Guid ReferenceKey
        {
            get { return referenceKey; }
        }

        private Color color = new Color();
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        private RatingModule parentModule;
        public RatingModule ParentModule
        {
            get { return parentModule; }
            set { parentModule = value; }
        }

        public int NumGamesOwned
        {
            get { return ((RatingModuleGame)ParentModule).GetGamesOnPlatform(this).Count(); }
        }

        public int NumGamesFinishable
        {
            get { return 0; }
        }

        public double AverageScoreOfGames { get { return 0; } }

        public double HighestScoreFromGames { get { return 0; } }

        public double LowestScoreFromGames { get { return 0; } }

        public double NumGamesFinished { get { return 0; } }

        public double PercentageGamesFinished
        {
            get
            {
                int numFinishable = NumGamesFinishable;
                if (numFinishable <= 0) numFinishable = 1;
                return NumGamesFinished / numFinishable * 100;
            }
        }

        public Platform() { }

        public Platform(RatingModule parentModule)
        {
            this.parentModule = parentModule;
        }

        public SavableRepresentation LoadIntoRepresentation()
        {
            SavableRepresentation sr = new SavableRepresentation();
            sr.SaveValue("referenceKey", referenceKey);
            sr.SaveValue("name", name);
            sr.SaveValue("color", color);
            return sr;
        }

        public void RestoreFromRepresentation(SavableRepresentation sr)
        {
            if (sr == null) return;
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "referenceKey":
                        referenceKey = sr.GetGuid(key);
                        break;
                    case "name":
                        name = sr.GetString(key);
                        break;
                    case "color":
                        color = sr.GetColor(key);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("Platform.cs RestoreFromRepresentation: unrecognized key " + key);
                        break;
                }
            }
        }

        public override int GetHashCode()
        {
            return ReferenceKey.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Platform o = (Platform)obj;
                return !ReferenceKey.Equals(Guid.Empty) && ReferenceKey.Equals(o.ReferenceKey);
            }
        }
    }
}
