using GameTracker;
using GameTracker.Sorting;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.Exceptions;
using RatableTracker.ListManipulation.Sorting;
using RatableTracker.ObjAddOns;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class EditCompilationViewModel : BaseViewModelEdit<GameCompilation>
    {
        private string _originalName = "";

        public Status Status
        {
            get => Item.StatusExtension.Status;
            set => SetProperty(Item.StatusExtension.Status, value, () => Item.StatusExtension.Status = value);
        }

        public IEnumerable<Status> CompletionStatuses
        {
            get => Module.StatusExtension.GetStatusList()
                .OfType<StatusGame>()
                .Where(s => (Item.IsUnfinishable && s.StatusUsage == StatusUsage.UnfinishableGamesOnly) ||
                            (!Item.IsUnfinishable && s.StatusUsage == StatusUsage.FinishableGamesOnly) ||
                            s.StatusUsage == StatusUsage.AllGames)
                .OrderBy(s => s.Name)
                .ToList();
        }

        public GameTracker.Platform Platform
        {
            get => Item.Platform;
            set => SetProperty(Item.Platform, value, () => Item.Platform = value);
        }

        public GameTracker.Platform PlatformPlayedOn
        {
            get => Item.PlatformPlayedOn;
            set => SetProperty(Item.PlatformPlayedOn, value, () => Item.PlatformPlayedOn = value);
        }

        public IEnumerable<GameTracker.Platform> Platforms
        {
            get => Module.GetPlatformList(new SortEngine() { SortOption = new SortOptionPlatformName() }, Settings);
        }

        public string GameComment
        {
            get => Item.GameComment;
            set => SetProperty(Item.GameComment, value, () => Item.GameComment = value);
        }

        public int MaxLengthName => GameCompilation.MaxLengthName;
        public int MaxLengthGameComment => GameCompilation.MaxLengthGameComment;

        public EditCompilationViewModel(IServiceProvider provider) : base(provider) { }

        protected override GameCompilation CreateNewObject()
        {
            return new GameCompilation(Settings, Module);
        }

        protected override GameCompilation CreateCopyObject(GameCompilation item)
        {
            return new GameCompilation(item);
        }

        protected override void UpdatePropertiesOnLoad()
        {
            Title = "Edit Compilation";
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(Platform));
            OnPropertyChanged(nameof(PlatformPlayedOn));
            OnPropertyChanged(nameof(GameComment));
        }

        protected override IList<GameCompilation> GetObjectList()
        {
            return Module.GetModelObjectList(Settings).OfType<GameCompilation>().ToList();
        }

        protected override void PreSave()
        {
            base.PreSave();
            GameComment = GameComment.Trim();
        }

        protected override async Task SaveObject()
        {
            if (Name.Length > 0 && !Name.Equals(_originalName))
            {
                var matches = GetObjectList().Where(c => c.Name.ToLower().Equals(Name.ToLower())).ToList();
                if (matches.Count > 0)
                    throw new ValidationException("A compilation with that name already exists");
            }
            await base.SaveObject();
        }

        protected override void PostLoad(GameCompilation item)
        {
            _originalName = item.Name;
        }
    }
}
