using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ScoreRanges;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class NewScoreRangeViewModel : BaseViewModel
    {
        private ScoreRange _item = new ScoreRange(SharedDataService.Module, SharedDataService.Settings);

        public string ItemId
        {
            get => _item.UniqueID.ToString();
            set
            {
                UniqueID key = UniqueID.Parse(value);
                LoadItemId(key);
            }
        }

        public ScoreRange Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
                Title = "Edit Score Range";
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(ScoreRelationship));
                Value1 = _item.ValueList.Count() >= 1 ? _item.ValueList.ElementAt(0) : 0;
                Value2 = _item.ValueList.Count() >= 2 ? _item.ValueList.ElementAt(1) : 0;
                OnPropertyChanged(nameof(Color));
            }
        }

        private double val1 = 0;
        private double val2 = 0;

        public string Name
        {
            get => Item.Name;
            set => SetProperty(Item.Name, value, () => Item.Name = value);
        }

        public ScoreRelationship ScoreRelationship
        {
            get => Item.ScoreRelationship;
            set => SetProperty(Item.ScoreRelationship, value, () => Item.ScoreRelationship = value);
        }

        public double Value1
        {
            get => val1;
            set => SetProperty(ref val1, value);
        }

        public double Value2
        {
            get => val2;
            set => SetProperty(ref val2, value);
        }

        public Microsoft.Maui.Graphics.Color Color
        {
            get => Item.Color.ToMAUIColor();
            set => SetProperty(Item.Color, value.ToFrameworkColor(), () => Item.Color = value.ToFrameworkColor());
        }

        public IEnumerable<ScoreRelationship> ScoreRelationships
        {
            get => SharedDataService.Module.GetScoreRelationshipList();
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public NewScoreRangeViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
            Title = "New Score Range";
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(Name) && ScoreRelationship != null;
        }

        private async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            List<double> values = new List<double>();
            if (ScoreRelationship.NumValuesRequired >= 1)
                values.Add(Value1);
            if (ScoreRelationship.NumValuesRequired >= 2)
                values.Add(Value2);
            Item.ValueList = values;

            try
            {
                Item.Save(SharedDataService.Module, SharedDataService.Settings);
            }
            catch (Exception ex)
            {
                var popup = new PopupMain("Unable to Save", ex.Message, PopupMain.EnumInputType.Ok);
                await ShowPopupAsync(popup);
                return;
            }

            await Shell.Current.GoToAsync("..");
        }

        public void LoadItemId(UniqueID itemId)
        {
            try
            {
                Item = RatableTracker.Util.Util.FindObjectInList(SharedDataService.Module.GetScoreRangeList(), itemId);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
