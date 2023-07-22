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
            MinScore = SharedDataService.Settings.MinScore.ToString();
            MaxScore = SharedDataService.Settings.MaxScore.ToString();
            ShowScoreNullStatus = SharedDataService.Settings.ShowScoreWhenNullStatus;
            TreatAllGamesAsOwned = SharedDataService.Settings.TreatAllGamesAsOwned;
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

            if (min != SharedDataService.Settings.MinScore || max != SharedDataService.Settings.MaxScore)
            {
                var popup = new PopupMain("Confirmation", "Changing the score ranges will scale all your existing scores to fit within the new range. Would you like to do this?", PopupMain.EnumInputType.YesNo)
                {
                    Size = new Size(300, 200)
                };
                var ret = (Tuple<PopupMain.EnumOutputType, string>)await UtilMAUI.ShowPopupAsync(popup);

                if (ret is null || ret.Item1 != PopupMain.EnumOutputType.Yes)
                {
                    return;
                }
            }

            SharedDataService.Settings.MinScore = min;
            SharedDataService.Settings.MaxScore = max;
            SharedDataService.Settings.ShowScoreWhenNullStatus = ShowScoreNullStatus;
            SharedDataService.Settings.TreatAllGamesAsOwned = TreatAllGamesAsOwned;
            try
            {
                SharedDataService.Settings.Save(SharedDataService.Module, SharedDataService.Settings);
            }
            catch (Exception ex)
            {
                await UtilMAUI.ShowPopupMainAsync("Unable to Save", ex.Message, PopupMain.EnumInputType.Ok);
                return;
            }

            await Shell.Current.GoToAsync("..");
        }

        private void UpdateAWSButtonText()
        {
            AWSButtonText = FileHandlerAWSS3.KeyFileExists(SharedDataService.PathController) ? "Switch back to local save files" : "Switch to remote save files with AWS";
        }

        private async void OnAWS()
        {
            if (FileHandlerAWSS3.KeyFileExists(SharedDataService.PathController))
            {
                // Remove key file
                await Task.Delay(500); // without delay, popup won't open
                var popup = new PopupMain("Overwrite local?", "Switch back to local has started. Transfer AWS save files to local? This will overwrite anything currently on this device.", PopupMain.EnumInputType.YesNo)
                {
                    Size = new Size(300, 200)
                };
                var ret = (Tuple<PopupMain.EnumOutputType, string>)await UtilMAUI.ShowPopupAsync(popup);
                
                IFileHandler newFileHandler = new FileHandlerLocalAppData(SharedDataService.PathController, LoadSaveMethodJSON.SAVE_FILE_DIRECTORY);
                if (ret is not null && ret.Item1 == PopupMain.EnumOutputType.Yes)
                {
                    ILoadSaveHandler<ILoadSaveMethodGame> newLoadSave = new LoadSaveHandler<ILoadSaveMethodGame>(() => new LoadSaveMethodJSONGame(newFileHandler, SharedDataService.Factory, App.Logger));
                    GameModule newModule = new GameModule(newLoadSave, App.Logger);
                    App.Logger.Log("Starting transfer from AWS to local");
                    SharedDataService.Module.TransferToNewModule(newModule, SharedDataService.Settings);
                }
                FileHandlerAWSS3.DeleteKeyFile(SharedDataService.PathController);
                SharedDataService.ResetSharedObjects();
            }
            else
            {
                // Add a key file
                var fileSelected = await FilePicker.PickAsync();
                if (fileSelected != null)
                {
                    await Task.Delay(500); // without delay, popup won't open
                    var popup = new PopupMain("Overwrite AWS?", "Switch to AWS has started. Transfer local save files to AWS? This will overwrite anything currently on your AWS account.", PopupMain.EnumInputType.YesNo)
                    {
                        Size = new Size(300, 200)
                    };
                    var ret = (Tuple<PopupMain.EnumOutputType, string>)await UtilMAUI.ShowPopupAsync(popup);

                    try
                    {
                        IFileHandler newFileHandler = new FileHandlerAWSS3(fileSelected.FullPath, SharedDataService.PathController);
                        if (ret is not null && ret.Item1 == PopupMain.EnumOutputType.Yes)
                        {
                            ILoadSaveHandler<ILoadSaveMethodGame> newLoadSave = new LoadSaveHandler<ILoadSaveMethodGame>(() => new LoadSaveMethodJSONGame(newFileHandler, SharedDataService.Factory, App.Logger));
                            GameModule newModule = new GameModule(newLoadSave, App.Logger);
                            App.Logger.Log("Starting transfer from local to AWS");
                            SharedDataService.Module.TransferToNewModule(newModule, SharedDataService.Settings);
                        }
                        SharedDataService.ResetSharedObjects();
                    }
                    catch (Exception ex)
                    {
                        await UtilMAUI.ShowPopupMainAsync("Error", "Something went wrong.\n" + ex.Message, PopupMain.EnumInputType.Ok);
                        FileHandlerAWSS3.DeleteKeyFile(SharedDataService.PathController);
                    }
                }
            }
            UpdateAWSButtonText();
        }
    }
}
