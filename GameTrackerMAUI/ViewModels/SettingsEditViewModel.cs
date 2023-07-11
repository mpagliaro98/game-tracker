using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
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
            MinScore = SharedDataService.Settings.MinScore.ToString();
            MaxScore = SharedDataService.Settings.MaxScore.ToString();
            //AWSButtonText = ContentLoadSaveAWSS3.KeyFileExists() ? "Switch back to local save files" : "Switch to remote save files with AWS";
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
                Tuple<PopupMain.EnumOutputType, string> ret = (Tuple<PopupMain.EnumOutputType, string>)await ShowPopupAsync(popup);

                if (ret is null || ret.Item1 != PopupMain.EnumOutputType.Yes)
                {
                    return;
                }
            }

            SharedDataService.Settings.MinScore = min;
            SharedDataService.Settings.MaxScore = max;
            try
            {
                SharedDataService.Settings.Save(SharedDataService.Module, SharedDataService.Settings);
            }
            catch (Exception ex)
            {
                var popup = new PopupMain("Unable to Save", ex.Message, PopupMain.EnumInputType.Ok);
                await ShowPopupAsync(popup);
                return;
            }

            await Shell.Current.GoToAsync("..");
        }

        private async void OnAWS()
        {
            //if (ContentLoadSaveAWSS3.KeyFileExists())
            //{
            //    // Remove key file
            //    var result = await Util.ShowPopupAsync("Overwrite local?", "Switch back to local has started. Transfer AWS save files to local? This will overwrite anything currently on this device.", PopupViewModel.EnumInputType.YesNo);

            //    if (result.Item1.ToString().ToUpper() == "YES")
            //    {
            //        IContentLoadSave<string, string> cls = new ContentLoadSaveLocal();
            //        IContentLoadSave<string, string> from = new ContentLoadSaveAWSS3();
            //        await ModuleService.GetActiveModule().TransferSaveFilesAsync(from, cls);
            //    }
            //    ContentLoadSaveAWSS3.DeleteKeyFile();
            //    ModuleService.ResetActiveModule();
            //    ModuleService.GetActiveModule();
            //}
            //else
            //{
            //    // Add a key file
            //    var fileSelected = await FilePicker.PickAsync();
            //    if (fileSelected != null)
            //    {
            //        var result = await Util.ShowPopupAsync("Overwrite AWS?", "Switch to AWS has started. Transfer local save files to AWS? This will overwrite anything currently on your AWS account.", PopupViewModel.EnumInputType.YesNo);
            //        try
            //        {
            //            ContentLoadSaveAWSS3.CreateKeyFile(fileSelected.FullPath);
            //            if (result.Item1.ToString().ToUpper() == "YES")
            //            {
            //                IContentLoadSave<string, string> cls = new ContentLoadSaveAWSS3();
            //                IContentLoadSave<string, string> from = new ContentLoadSaveLocal();
            //                await ModuleService.GetActiveModule().TransferSaveFilesAsync(from, cls);
            //            }
            //            ModuleService.ResetActiveModule();
            //            ModuleService.GetActiveModule();
            //        }
            //        catch (Exception ex)
            //        {
            //            await Util.ShowPopupAsync("Error", "Something went wrong.\n" + ex.Message, PopupViewModel.EnumInputType.Ok);
            //            ContentLoadSaveAWSS3.DeleteKeyFile();
            //        }
            //    }
            //}
            //AWSButtonText = ContentLoadSaveAWSS3.KeyFileExists() ? "Switch back to local save files" : "Switch to remote save files with AWS";
        }
    }
}
