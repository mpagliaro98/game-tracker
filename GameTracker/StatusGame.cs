using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Model;
using RatableTracker.ObjAddOns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTracker
{
    public enum StatusUsage : int
    {
        UnfinishableGamesOnly = 1,
        FinishableGamesOnly = 2,
        AllGames = 3
    }

    public class StatusGame : Status
    {
        [Savable] public bool UseAsFinished { get; set; } = false;
        [Savable] public bool HideScoreFromList { get; set; } = false;
        [Savable(HandleLoadManually = true, HandleRestoreManually = true)] public StatusUsage StatusUsage { get; set; } = StatusUsage.AllGames;

        public override bool HideScoreOfModelObject
        {
            get { return HideScoreFromList; }
        }

        protected new SettingsGame Settings => (SettingsGame)base.Settings;

        public StatusGame(IModuleStatus module, SettingsGame settings) : base(module, settings) { }

        public StatusGame(StatusGame copyFrom) : base(copyFrom)
        {
            UseAsFinished = copyFrom.UseAsFinished;
            HideScoreFromList = copyFrom.HideScoreFromList;
            StatusUsage = copyFrom.StatusUsage;
        }

        protected override void LoadHandleManually(ref SavableRepresentation sr, string key)
        {
            base.LoadHandleManually(ref sr, key);
            if (key == "StatusUsage") sr.SaveValue(key, new ValueContainer(StatusUsage.ToString()));
        }

        protected override void RestoreHandleManually(SavableRepresentation sr, string key)
        {
            base.RestoreHandleManually(sr, key);
            switch (key)
            {
                case "StatusUsage":
                    StatusUsage = Enum.Parse<StatusUsage>(sr.GetValue(key).GetString());
                    break;
            }
        }
    }
}
