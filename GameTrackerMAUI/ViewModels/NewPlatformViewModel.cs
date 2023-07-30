using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class NewPlatformViewModel : BaseViewModelEdit<GameTracker.Platform>
    {
        public string Abbreviation
        {
            get => Item.Abbreviation;
            set => SetProperty(Item.Abbreviation, value, () => Item.Abbreviation = value);
        }

        public int ReleaseYear
        {
            get => Item.ReleaseYear;
            set => SetProperty(Item.ReleaseYear, value, () => Item.ReleaseYear = value);
        }

        public int AcquiredYear
        {
            get => Item.AcquiredYear;
            set => SetProperty(Item.AcquiredYear, value, () => Item.AcquiredYear = value);
        }

        public Microsoft.Maui.Graphics.Color Color
        {
            get => Item.Color.ToMAUIColor();
            set => SetProperty(Item.Color.ToMAUIColor(), value, () => Item.Color = value.ToFrameworkColor());
        }

        public NewPlatformViewModel(IServiceProvider provider) : base(provider)
        {
            Title = "New Platform";
        }

        protected override GameTracker.Platform CreateNewObject()
        {
            return new GameTracker.Platform(Module, Settings);
        }

        protected override GameTracker.Platform CreateCopyObject(GameTracker.Platform item)
        {
            return new GameTracker.Platform(item);
        }

        protected override void UpdatePropertiesOnLoad()
        {
            Title = "Edit Platform";
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Abbreviation));
            OnPropertyChanged(nameof(ReleaseYear));
            OnPropertyChanged(nameof(AcquiredYear));
            OnPropertyChanged(nameof(Color));
        }

        protected override IList<GameTracker.Platform> GetObjectList()
        {
            throw new NotImplementedException();
        }
    }
}
