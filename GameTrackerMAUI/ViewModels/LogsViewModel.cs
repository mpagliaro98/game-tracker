using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrackerMAUI.ViewModels
{
    public partial class LogsViewModel : BaseViewModel
    {
        public ObservableCollection<RatableTracker.Util.FileInfo> Items => new(Logger.EnumerateLogFiles());

        private RatableTracker.Util.FileInfo _selectedLogFile;
        public RatableTracker.Util.FileInfo SelectedLogFile
        {
            get => _selectedLogFile;
            set => SetProperty(ref _selectedLogFile, value);
        }

        private readonly IFileSaver fileSaver;

        public LogsViewModel(IServiceProvider provider, IFileSaver fileSaver) : base(provider)
        {
            this.fileSaver = fileSaver;
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            SelectedLogFile = null;
        }

        [RelayCommand]
        private async Task ItemSelected(object item)
        {
            if (item != null)
            {
                var fileInfo = item as RatableTracker.Util.FileInfo;
                string logContent = Logger.GetLogFileContents(((RatableTracker.Util.FileInfo)item).Name);
                using var stream = new MemoryStream(RatableTracker.Util.Util.TextEncoding.GetBytes(logContent));
                var result = await fileSaver.SaveAsync(fileInfo.Name, stream, new CancellationToken());
                if (result.IsSuccessful)
                {
                    await ToastService.ShowToastAsync("File saved: " + result.FilePath);
                    Logger.Log("Log file \"" + fileInfo.Name + "\" successfully downloaded to: " + result.FilePath);
                }
                else
                {
                    Logger.Log("Unable to download log file \"" + fileInfo.Name + "\": " + result.Exception.Message);
                }
                SelectedLogFile = null;
            }
        }
    }
}
