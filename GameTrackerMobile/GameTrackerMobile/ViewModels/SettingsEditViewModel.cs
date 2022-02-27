using GameTrackerMobile.Services;
using RatableTracker.Framework.Interfaces;
using RatableTracker.Framework.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Essentials;
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
        private string awsButtonText = "Switch to remote save files with AWS";

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

        public string AWSButtonText
        {
            get => awsButtonText;
            set => SetProperty(ref awsButtonText, value);
        }

        public Command SaveCommand { get; }
        public Command AWSCommand { get; }

        public SettingsEditViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            AWSCommand = new Command(OnAWS);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
            SetValues();
        }

        private void SetValues()
        {
            var rm = ModuleService.GetActiveModule();
            MinScore = rm.Settings.MinScore.ToString();
            MaxScore = rm.Settings.MaxScore.ToString();
            AWSButtonText = ContentLoadSaveAWSS3.KeyFileExists() ? "Switch back to local save files" : "Switch to remote save files with AWS";
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
            double min = double.Parse(MinScore);
            double max = double.Parse(MaxScore);

            if (min != rm.Settings.MinScore || max != rm.Settings.MaxScore)
            {
                var result = await Util.ShowPopupAsync("Confirmation", "Changing the score ranges will scale all your existing scores to fit within the new range. Would you like to do this?", PopupViewModel.EnumInputType.YesNo);

                if (result.Item1.ToString().ToUpper() != "YES")
                {
                    return;
                }
            }

            rm.SetScoresAndUpdate(min, max);

            await Shell.Current.GoToAsync("..");
        }

        private async void OnAWS()
        {
            if (ContentLoadSaveAWSS3.KeyFileExists())
            {
                // Remove key file
                var result = await Util.ShowPopupAsync("Overwrite local?", "Switch back to local has started. Transfer AWS save files to local? This will overwrite anything currently on this device.", PopupViewModel.EnumInputType.YesNo);

                if (result.Item1.ToString().ToUpper() == "YES")
                {
                    IContentLoadSave<string, string> cls = new ContentLoadSaveLocal();
                    IContentLoadSave<string, string> from = new ContentLoadSaveAWSS3();
                    await ModuleService.GetActiveModule().TransferSaveFilesAsync(from, cls);
                }
                ContentLoadSaveAWSS3.DeleteKeyFile();
                ModuleService.ResetActiveModule();
                ModuleService.GetActiveModule();
            }
            else
            {
                // Add a key file
                var fileSelected = await FilePicker.PickAsync();
                if (fileSelected != null)
                {
                    var result = await Util.ShowPopupAsync("Overwrite AWS?", "Switch to AWS has started. Transfer local save files to AWS? This will overwrite anything currently on your AWS account.", PopupViewModel.EnumInputType.YesNo);
                    try
                    {
                        ContentLoadSaveAWSS3.CreateKeyFile(fileSelected.FullPath);
                        if (result.Item1.ToString().ToUpper() == "YES")
                        {
                            IContentLoadSave<string, string> cls = new ContentLoadSaveAWSS3();
                            IContentLoadSave<string, string> from = new ContentLoadSaveLocal();
                            await ModuleService.GetActiveModule().TransferSaveFilesAsync(from, cls);
                        }
                        ModuleService.ResetActiveModule();
                        ModuleService.GetActiveModule();
                    }
                    catch (Exception ex)
                    {
                        await Util.ShowPopupAsync("Error", "Something went wrong.\n" + ex.Message, PopupViewModel.EnumInputType.Ok);
                        ContentLoadSaveAWSS3.DeleteKeyFile();
                    }
                }
            }
            AWSButtonText = ContentLoadSaveAWSS3.KeyFileExists() ? "Switch back to local save files" : "Switch to remote save files with AWS";
        }
    }
}
