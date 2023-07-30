using CommunityToolkit.Maui.Views;
using GameTracker;
using GameTrackerMAUI.Model;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public GameModule Module => provider.GetSharedDataService().Module;
        public SettingsGame Settings => provider.GetSharedDataService().Settings;
        public ILogger Logger => provider.GetLogger();
        public IPathController PathController => provider.GetPathController();
        public ILoadSaveHandler<ILoadSaveMethodGame> LoadSave => provider.GetSharedDataService().LoadSave;
        public GameTrackerFactory Factory => provider.GetFactory();
        public ISavedState SavedState => provider.GetSavedState();

        protected readonly IServiceProvider provider;

        public BaseViewModel(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public virtual void OnAppearing()
        {

        }

        protected bool SetProperty<TProp>(TProp backingStore, TProp value, Action setProp,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            // don't continue if the value didn't actually change
            if (EqualityComparer<TProp>.Default.Equals(backingStore, value))
                return false;

            setProp();
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<TProp>(ref TProp backingStore, TProp value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            // don't continue if the value didn't actually change
            if (EqualityComparer<TProp>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
