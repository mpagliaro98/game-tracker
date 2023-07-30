using GameTrackerMAUI.Model;
using GameTrackerMAUI.Views;
using RatableTracker.ListManipulation.Filtering;
using RatableTracker.ListManipulation.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    public abstract class BaseViewModelListSortSearch<T> : BaseViewModelList<T>
    {
        public bool FromFilterPage { get; set; } = false;
        public Command SortCommand { get; }
        public Command SortDirectionCommand { get; }
        public Command SearchCommand { get; }

        private string _sortDirectionButtonText = "Asc";
        public string SortDirectionButtonText
        {
            get => _sortDirectionButtonText;
            set => SetProperty(ref _sortDirectionButtonText, value);
        }

        private string _sortDirectionImageName = "sort_ascending";
        public string SortDirectionImageName
        {
            get => _sortDirectionImageName;
            set => SetProperty(ref _sortDirectionImageName, value);
        }

        private string _sortImageName = "sort";
        public string SortImageName
        {
            get => _sortImageName;
            set => SetProperty(ref _sortImageName, value);
        }

        private string _searchImageName = "search";
        public string SearchImageName
        {
            get => _searchImageName;
            set => SetProperty(ref _searchImageName, value);
        }

        protected abstract FilterEngine FilterObject { get; }
        protected abstract SortEngine SortObject { get; }
        protected abstract FilterType FilterType { get; }

        public BaseViewModelListSortSearch(IServiceProvider provider) : base(provider)
        {
            SortCommand = new Command(OnSort);
            SearchCommand = new Command(OnSearch);
            SortDirectionCommand = new Command(OnSortDirection);
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            SetSortDirectionButton();
            SetSortButton();
            SetSearchButton();
        }

        protected override async Task PostLoadAsync()
        {
            SetSearchButton();
            if (FromFilterPage)
            {
                var count = Items.Count;
                await ToastService.ShowToastAsync($"{count} item{(count != 1 ? "s" : "")} found");
                FromFilterPage = false;
            }
        }

        private async void OnSort()
        {
            IList<ISortOption> sortOptions = SortEngine.GetSortOptionList<T>(Module, Settings);
            List<PopupListOption> options = sortOptions.Select(so => new PopupListOption(so, so.Name)).ToList();

            ISortOption selectedValue = SortObject.SortOption;
            var ret = await UtilMAUI.ShowPopupListAsync("Sort by", options, selectedValue);
            if (ret != null && ret.Item1 == PopupList.EnumOutputType.Selection)
            {
                if (ret.Item2 is null)
                    return;
                else if (ret.Item2.Equals(SortObject.SortOption))
                    SortObject.SortOption = null;
                else
                    SortObject.SortOption = (SortOptionBase)ret.Item2;
                SavedState.Save(PathController);

                IsBusy = true;
                SetSortButton();
            }
        }

        private void OnSortDirection()
        {
            SortObject.SortMode = SortObject.SortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;
            SetSortDirectionButton();
            SavedState.Save(PathController);
            IsBusy = true;
        }

        private void SetSortDirectionButton()
        {
            SortDirectionButtonText = SortObject.SortMode == SortMode.Ascending ? "Asc" : "Desc";
            SortDirectionImageName = SortObject.SortMode == SortMode.Ascending ? "sort_ascending" : "sort_descending";
        }

        private void SetSortButton()
        {
            SortImageName = SortObject.SortOption == null ? "sort" : "sort_active";
        }

        private void SetSearchButton()
        {
            SearchImageName = FilterObject.Filters.Count <= 0 ? "search" : "search_active";
        }

        private async void OnSearch()
        {
            await Shell.Current.GoToAsync($"{nameof(FilterPage)}?{nameof(FilterViewModel.FilterTypeParam)}={FilterType}");
        }
    }
}
