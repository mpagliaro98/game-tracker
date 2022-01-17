using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RatableTracker.Framework.ObjectHierarchy;
using RatableTracker.Framework.LoadSave;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework;

namespace GameTracker.Model
{
    public class RatableGame : RatableObjectStatusCategorical
    {
        public static int MaxLengthCompletionCriteria => 1000;
        public static int MaxLengthCompletionComment => 1000;
        public static int MaxLengthTimeSpent => 1000;

        private ObjectReference platform = new ObjectReference();
        public ObjectReference RefPlatform => platform;

        private ObjectReference platformPlayedOn = new ObjectReference();
        public ObjectReference RefPlatformPlayedOn => platformPlayedOn;

        public string CompletionCriteria { get; set; } = "";

        public string CompletionComment { get; set; } = "";

        public string TimeSpent { get; set; } = "";

        public DateTime AcquiredOn { get; set; } = DateTime.MinValue;

        public DateTime StartedOn { get; set; } = DateTime.MinValue;

        public DateTime FinishedOn { get; set; } = DateTime.MinValue;

        public bool IsRemaster { get; set; } = false;

        private ObjectReference originalGame = new ObjectReference();
        public ObjectReference RefOriginalGame => originalGame;

        public bool UseOriginalGameScore { get; set; } = false;

        public RatableGame() : base() { }

        public override SavableRepresentation<T> LoadIntoRepresentation<T>()
        {
            SavableRepresentation<T> sr = base.LoadIntoRepresentation<T>();
            sr.SaveValue("platform", platform);
            sr.SaveValue("platformPlayedOn", platformPlayedOn);
            sr.SaveValue("completionCriteria", CompletionCriteria);
            sr.SaveValue("completionComment", CompletionComment);
            sr.SaveValue("timeSpent", TimeSpent);
            sr.SaveValue("acquiredOn", AcquiredOn);
            sr.SaveValue("startedOn", StartedOn);
            sr.SaveValue("finishedOn", FinishedOn);
            sr.SaveValue("isRemaster", IsRemaster);
            sr.SaveValue("originalGame", originalGame);
            sr.SaveValue("useOriginalGameScore", UseOriginalGameScore);
            return sr;
        }

        public override void RestoreFromRepresentation<T>(SavableRepresentation<T> sr)
        {
            base.RestoreFromRepresentation(sr);
            foreach (string key in sr.GetAllSavedKeys())
            {
                switch (key)
                {
                    case "platform":
                        platform = sr.GetISavable<ObjectReference>(key);
                        break;
                    case "platformPlayedOn":
                        platformPlayedOn = sr.GetISavable<ObjectReference>(key);
                        break;
                    case "completionCriteria":
                        CompletionCriteria = sr.GetString(key);
                        break;
                    case "completionComment":
                        CompletionComment = sr.GetString(key);
                        break;
                    case "timeSpent":
                        TimeSpent = sr.GetString(key);
                        break;
                    case "acquiredOn":
                        AcquiredOn = sr.GetDateTime(key);
                        break;
                    case "startedOn":
                        StartedOn = sr.GetDateTime(key);
                        break;
                    case "finishedOn":
                        FinishedOn = sr.GetDateTime(key);
                        break;
                    case "isRemaster":
                        IsRemaster = sr.GetBool(key);
                        break;
                    case "originalGame":
                        originalGame = sr.GetISavable<ObjectReference>(key);
                        break;
                    case "useOriginalGameScore":
                        UseOriginalGameScore = sr.GetBool(key);
                        break;
                    default:
                        break;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public void SetPlatform<T>(T obj) where T : Platform, IReferable
        {
            platform.SetReference(obj);
        }

        public void RemovePlatform()
        {
            platform.ClearReference();
        }

        public void SetPlatformPlayedOn<T>(T obj) where T : Platform, IReferable
        {
            platformPlayedOn.SetReference(obj);
        }

        public void RemovePlatformPlayedOn()
        {
            platformPlayedOn.ClearReference();
        }

        public void SetOriginalGame<T>(T obj) where T : RatableGame, IReferable
        {
            originalGame.SetReference(obj);
        }

        public void RemoveOriginalGame()
        {
            originalGame.ClearReference();
        }
    }
}
