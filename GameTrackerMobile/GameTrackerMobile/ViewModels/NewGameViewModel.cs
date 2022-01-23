using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using GameTracker.Model;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    public class NewGameViewModel : BaseViewModel<RatableGame>
    {
        private string name;

        public NewGameViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(name);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            RatableGame newItem = new RatableGame()
            {
                Name = Name
            };

            await DataStore.AddItemAsync(newItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
