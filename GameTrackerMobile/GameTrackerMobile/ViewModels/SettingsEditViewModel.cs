using GameTrackerMobile.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    public class SettingsEditViewModel
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

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string minScore;
        private string maxScore;

        public string MinScore
        {
            get => minScore;
            set => SetProperty(ref minScore, value);
        }

        public string MaxScore
        {
            get => maxScore;
            set => SetProperty(ref maxScore, value);
        }

        public Command SaveCommand { get; }

        public SettingsEditViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
            SetValues();
        }

        private void SetValues()
        {
            var rm = ModuleService.GetActiveModule();
            MinScore = rm.Settings.MinScore.ToString();
            MaxScore = rm.Settings.MaxScore.ToString();
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(minScore) &&
                !String.IsNullOrWhiteSpace(maxScore) &&
                double.TryParse(minScore, out _) &&
                double.TryParse(maxScore, out _);
        }

        private async void OnSave()
        {
            var rm = ModuleService.GetActiveModule();
            rm.Settings.MinScore = double.Parse(MinScore);
            rm.Settings.MaxScore = double.Parse(MaxScore);
            rm.SaveSettings();

            await Shell.Current.GoToAsync("..");
        }
    }
}
