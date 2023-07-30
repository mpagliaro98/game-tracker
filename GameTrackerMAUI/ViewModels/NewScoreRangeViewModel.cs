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
    public class NewScoreRangeViewModel : BaseViewModelEdit<ScoreRange>
    {
        private double val1 = 0;
        private double val2 = 0;

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
            get => Module.GetScoreRelationshipList();
        }

        public NewScoreRangeViewModel(IServiceProvider provider) : base(provider)
        {
            Title = "New Score Range";
        }

        protected override bool ValidateSave()
        {
            return base.ValidateSave() && ScoreRelationship != null;
        }

        protected override void PreSave()
        {
            List<double> values = new();
            if (ScoreRelationship.NumValuesRequired >= 1)
                values.Add(Value1);
            if (ScoreRelationship.NumValuesRequired >= 2)
                values.Add(Value2);
            Item.ValueList = values;
        }

        protected override ScoreRange CreateNewObject()
        {
            return new ScoreRange(Module, Settings);
        }

        protected override ScoreRange CreateCopyObject(ScoreRange item)
        {
            return new ScoreRange(item);
        }

        protected override void UpdatePropertiesOnLoad()
        {
            Title = "Edit Score Range";
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ScoreRelationship));
            Value1 = Item.ValueList.Count >= 1 ? Item.ValueList.ElementAt(0) : 0;
            Value2 = Item.ValueList.Count >= 2 ? Item.ValueList.ElementAt(1) : 0;
            OnPropertyChanged(nameof(Color));
        }

        protected override IList<ScoreRange> GetObjectList()
        {
            return Module.GetScoreRangeList();
        }
    }
}
