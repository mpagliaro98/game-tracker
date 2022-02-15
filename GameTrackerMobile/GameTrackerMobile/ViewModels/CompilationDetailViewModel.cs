﻿using GameTracker.Model;
using GameTrackerMobile.Services;
using GameTrackerMobile.Views;
using RatableTracker.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class CompilationDetailViewModel : BaseViewModel<GameCompilation>
    {
        private GameCompilation item = new GameCompilation();

        public Command EditCommand { get; }

        public GameCompilation Item
        {
            get => item;
            set
            {
                SetProperty(ref item, value);
                OnPropertyChanged("CompletionStatus");
                OnPropertyChanged("HasCompletionStatus");
                OnPropertyChanged("StatusMarkedAsFinished");
                OnPropertyChanged("Platform");
                OnPropertyChanged("HasPlatform");
                OnPropertyChanged("PlatformPlayedOn");
                OnPropertyChanged("HasPlatformPlayedOn");
                OnPropertyChanged("CategoryValues");
                OnPropertyChanged("FinalScore");
                OnPropertyChanged("FinalScoreColor");
                OnPropertyChanged("Stats");
                OnPropertyChanged("ShowCategoryValues");
            }
        }

        public string ItemId
        {
            get => new ObjectReference(item).ToString();
            set
            {
                ObjectReference key = (ObjectReference)value;
                LoadItemId(key);
            }
        }

        public CompletionStatus CompletionStatus
        {
            get => Item.RefStatus.HasReference() ? ModuleService.GetActiveModule().FindStatus(Item.RefStatus) : new CompletionStatus();
        }

        public bool HasCompletionStatus
        {
            get => Item.RefStatus.HasReference();
        }

        public bool StatusMarkedAsFinished
        {
            get => HasCompletionStatus ? CompletionStatus.UseAsFinished : false;
        }

        public Platform Platform
        {
            get => Item.RefPlatform.HasReference() ? ModuleService.GetActiveModule().FindPlatform(Item.RefPlatform) : new Platform();
        }

        public bool HasPlatform
        {
            get => Item.RefPlatform.HasReference();
        }

        public Platform PlatformPlayedOn
        {
            get => Item.RefPlatformPlayedOn.HasReference() ? ModuleService.GetActiveModule().FindPlatform(Item.RefPlatformPlayedOn) : new Platform();
        }

        public bool HasPlatformPlayedOn
        {
            get => Item.RefPlatformPlayedOn.HasReference();
        }

        public IEnumerable<CategoryValueContainer> CategoryValues
        {
            get
            {
                if (Item.Name == "")
                    return new List<CategoryValueContainer>();
                List<CategoryValueContainer> vals = new List<CategoryValueContainer>();
                var module = ModuleService.GetActiveModule();
                foreach (var cat in module.RatingCategories)
                {
                    var container = new CategoryValueContainer();
                    container.CategoryName = cat.Name;
                    container.CategoryValue = module.GetScoreOfCategory(Item, cat);
                    vals.Add(container);
                }
                return vals;
            }
        }

        public double FinalScore
        {
            get
            {
                if (Item.Name == "")
                    return ModuleService.GetActiveModule().Settings.MinScore;
                return ModuleService.GetActiveModule().GetScoreOfObject(Item);
            }
        }

        public Xamarin.Forms.Color FinalScoreColor
        {
            get
            {
                if (Item.Name == "")
                    return new Xamarin.Forms.Color();
                return ModuleService.GetActiveModule().GetRangeColorFromObject(Item).ToXamarinColor();
            }
        }

        public string Stats
        {
            get
            {
                if (Item.Name == "")
                    return "";
                var module = ModuleService.GetActiveModule();
                int rankOverall = module.GetRankOfScore(FinalScore, Item);
                int rankPlatform = -1;
                Platform platform = HasPlatform ? Platform : null;
                if (platform != null) rankPlatform = module.GetRankOfScoreByPlatform(FinalScore, platform, Item);

                string text = "";
                if (rankPlatform > 0) text += "#" + rankPlatform.ToString() + " on " + platform.Name + "\n";
                text += "#" + rankOverall.ToString() + " overall";
                return text;
            }
        }

        public bool ShowCategoryValues
        {
            get => !Item.IgnoreCategories && !Item.UseOriginalGameScore;
        }

        public CompilationDetailViewModel()
        {
            EditCommand = new Command(OnEdit);
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

        async void OnEdit()
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(EditCompilationPage)}?{nameof(EditCompilationViewModel.ItemId)}={new ObjectReference(item)}");
        }
    }
}
