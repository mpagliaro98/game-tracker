using GameTracker;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.Exceptions;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    public class SettingsEditViewModel : BaseViewModel
    {
        private string minScore;
        private string maxScore;
        private bool showScoreNullStatus;
        private bool treatAllGamesAsOwned;
        private bool unownedFinishCount;
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

        public bool ShowScoreNullStatus
        {
            get => showScoreNullStatus;
            set => SetProperty(ref showScoreNullStatus, value);
        }

        public bool TreatAllGamesAsOwned
        {
            get => treatAllGamesAsOwned;
            set => SetProperty(ref treatAllGamesAsOwned, value);
        }

        public bool UnownedFinishCount
        {
            get => unownedFinishCount;
            set => SetProperty(ref unownedFinishCount, value);
        }

        public string AWSButtonText
        {
            get => awsButtonText;
            set => SetProperty(ref awsButtonText, value);
        }

        public Command SaveCommand { get; }
        public Command AWSCommand { get; }

        public SettingsEditViewModel(IServiceProvider provider) : base(provider)
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            AWSCommand = new Command(OnAWS);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
            SetValues();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            SetValues();
        }

        private void SetValues()
        {
            MinScore = Settings.MinScore.ToString();
            MaxScore = Settings.MaxScore.ToString();
            ShowScoreNullStatus = Settings.ShowScoreWhenNullStatus;
            TreatAllGamesAsOwned = Settings.TreatAllGamesAsOwned;
            UnownedFinishCount = Settings.IncludeUnownedGamesInFinishCount;
            UpdateAWSButtonText();
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
            double min = double.Parse(MinScore);
            double max = double.Parse(MaxScore);

            if (min != Settings.MinScore || max != Settings.MaxScore)
            {
                if (!(await AlertService.DisplayConfirmationAsync("Confirmation", "Changing the score ranges will scale all your existing scores to fit within the new range. Would you like to do this?")))
                {
                    return;
                }
            }

            Settings.MinScore = min;
            Settings.MaxScore = max;
            Settings.ShowScoreWhenNullStatus = ShowScoreNullStatus;
            Settings.TreatAllGamesAsOwned = TreatAllGamesAsOwned;
            Settings.IncludeUnownedGamesInFinishCount = UnownedFinishCount;
            try
            {
                Settings.Save(Module, Settings);
            }
            catch (Exception ex)
            {
                await AlertService.DisplayAlertAsync("Unable to Save", ex.Message);
                return;
            }

            await Shell.Current.GoToAsync("..");
        }

        private void UpdateAWSButtonText()
        {
            AWSButtonText = FileHandlerAWSS3.KeyFileExists(PathController) ? "Switch back to local save files" : "Switch to remote save files with AWS";
        }

        private async void OnAWS()
        {
            Task t = null;
            if (FileHandlerAWSS3.KeyFileExists(PathController))
            {
                // Remove key file
                IFileHandler newFileHandler = new FileHandlerLocalAppData(PathController, LoadSaveMethodJSON.SAVE_FILE_DIRECTORY);
                if (await AlertService.DisplayConfirmationAsync("Overwrite local?", "Switch back to local has started. Transfer AWS save files to local? This will overwrite anything currently on this device."))
                {
                    ILoadSaveHandler<ILoadSaveMethodGame> newLoadSave = new LoadSaveHandler<ILoadSaveMethodGame>(() => new LoadSaveMethodJSONGame(newFileHandler, Factory, new Logger(Logger)));
                    GameModule newModule = new(newLoadSave, new Logger(Logger));
                    Logger.Log("Starting transfer from AWS to local");
                    Module.TransferToNewModule(newModule, Settings);
                }
                FileHandlerAWSS3.DeleteKeyFile(PathController);
                provider.GetSharedDataService().ResetSharedObjects();
                t = provider.GetSharedDataServiceAsync(); // start the load now
            }
            else
            {
                // Add a key file
                var fileSelected = await FilePicker.PickAsync();
                if (fileSelected != null)
                {
                    try
                    {
                        IFileHandler newFileHandler = new FileHandlerAWSS3(fileSelected.FullPath, PathController);
                        if (await AlertService.DisplayConfirmationAsync("Overwrite AWS?", "Switch to AWS has started. Transfer local save files to AWS? This will overwrite anything currently on your AWS account."))
                        {
                            ILoadSaveHandler<ILoadSaveMethodGame> newLoadSave = new LoadSaveHandler<ILoadSaveMethodGame>(() => new LoadSaveMethodJSONGame(newFileHandler, Factory, new Logger(Logger)));
                            GameModule newModule = new GameModule(newLoadSave, new Logger(Logger));
                            Logger.Log("Starting transfer from local to AWS");
                            Module.TransferToNewModule(newModule, Settings);
                        }
                        provider.GetSharedDataService().ResetSharedObjects();
                        t = provider.GetSharedDataServiceAsync(); // start the load now
                    }
                    catch (Exception ex)
                    {
                        await AlertService.DisplayAlertAsync("Error", "Something went wrong.\n" + ex.Message);
                        FileHandlerAWSS3.DeleteKeyFile(PathController);
                    }
                }
            }
            UpdateAWSButtonText();
            if (t != null) await t;
        }
    }
}
