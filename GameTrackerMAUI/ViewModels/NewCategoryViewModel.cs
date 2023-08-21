using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.ObjAddOns;
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
    public class NewCategoryViewModel : BaseViewModelEdit<RatingCategoryWeighted>
    {
        public double Weight
        {
            get => Item.Weight;
            set => SetProperty(Item.Weight, value, () => Item.Weight = value);
        }

        public string Comment
        {
            get => Item.Comment;
            set => SetProperty(Item.Comment, value, () => Item.Comment = value);
        }

        public int MaxLengthName => RatingCategoryWeighted.MaxLengthName;
        public int MaxLengthComment => RatingCategoryWeighted.MaxLengthComment;

        public NewCategoryViewModel(IServiceProvider provider) : base(provider)
        {
            Title = "New Category";
        }

        protected override RatingCategoryWeighted CreateNewObject()
        {
            return new RatingCategoryWeighted(Module, Settings);
        }

        protected override RatingCategoryWeighted CreateCopyObject(RatingCategoryWeighted item)
        {
            return new RatingCategoryWeighted(item);
        }

        protected override void UpdatePropertiesOnLoad()
        {
            Title = "Edit Category";
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Weight));
            OnPropertyChanged(nameof(Comment));
        }

        protected override void PreSave()
        {
            base.PreSave();
            Comment = Comment.Trim();
        }

        protected override IList<RatingCategoryWeighted> GetObjectList()
        {
            return Module.CategoryExtension.GetRatingCategoryList().OfType<RatingCategoryWeighted>().ToList();
        }
    }
}
