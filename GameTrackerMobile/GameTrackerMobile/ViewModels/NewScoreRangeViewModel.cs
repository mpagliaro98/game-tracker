using GameTrackerMobile.Services;
using RatableTracker.Framework;
using RatableTracker.Framework.Exceptions;
using RatableTracker.Framework.ScoreRelationships;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;
using System.Linq;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class NewScoreRangeViewModel : BaseViewModel<ScoreRange>
    {
        private ScoreRange item;

        public string ItemId
        {
            get => new ObjectReference(item).ToString();
            set
            {
                ObjectReference key = (ObjectReference)value;
                LoadItemId(key);
            }
        }

        public ScoreRange Item
        {
            get => item;
            set
            {
                SetProperty(ref item, value);
                Name = item.Name;
                ScoreRelationship = ModuleService.GetActiveModule().FindScoreRelationship(item.RefScoreRelationship);
                Value1 = item.ValueList.Count() >= 1 ? item.ValueList.ElementAt(0) : 0;
                Value2 = item.ValueList.Count() >= 2 ? item.ValueList.ElementAt(1) : 0;
                Color = item.Color.ToXamarinColor();
            }
        }

        private string name;
        private ScoreRelationship rel;
        private double val1;
        private double val2;
        private Xamarin.Forms.Color color;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public ScoreRelationship ScoreRelationship
        {
            get => rel;
            set => SetProperty(ref rel, value);
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

        public Xamarin.Forms.Color Color
        {
            get => color;
            set => SetProperty(ref color, value);
        }

        public IEnumerable<ScoreRelationship> ScoreRelationships
        {
            get => ModuleService.GetActiveModule().ScoreRelationships;
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public NewScoreRangeViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(name) && rel != null;
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
            ScoreRange newItem = new ScoreRange()
            {
                Name = Name,
                Color = Color.ToFrameworkColor(),
                ValueList = values
            };
            newItem.SetScoreRelationship(ScoreRelationship);

            try
            {
                ModuleService.GetActiveModule().ValidateRange(newItem);
            }
            catch (ValidationException e)
            {
                await Util.ShowPopupAsync("Error", e.Message, PopupViewModel.EnumInputType.Ok);
                return;
            }

            if (Item == null)
                await DataStore.AddItemAsync(newItem);
            else
            {
                newItem.OverwriteReferenceKey(Item);
                await DataStore.UpdateItemAsync(newItem);
            }

            await Shell.Current.GoToAsync("..");
        }

        public async void LoadItemId(ObjectReference itemId)
        {
            try
            {
                Item = await DataStore.GetItemAsync(itemId);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
