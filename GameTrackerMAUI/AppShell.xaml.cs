using CommunityToolkit.Maui.Storage;
using GameTrackerMAUI.Services;
using GameTrackerMAUI.Views;
using RatableTracker.Interfaces;

namespace GameTrackerMAUI
{
    public partial class AppShell : Shell
    {
        private IServiceProvider provider;

        public AppShell(IServiceProvider provider)
        {
            InitializeComponent();
            this.provider = provider;

            Routing.RegisterRoute(nameof(NewGamePage), typeof(NewGamePage));
            Routing.RegisterRoute(nameof(GameDetailPage), typeof(GameDetailPage));
            Routing.RegisterRoute(nameof(NewPlatformPage), typeof(NewPlatformPage));
            Routing.RegisterRoute(nameof(PlatformDetailPage), typeof(PlatformDetailPage));
            Routing.RegisterRoute(nameof(CompilationDetailPage), typeof(CompilationDetailPage));
            Routing.RegisterRoute(nameof(EditCompilationPage), typeof(EditCompilationPage));
            Routing.RegisterRoute(nameof(CategoryDetailPage), typeof(CategoryDetailPage));
            Routing.RegisterRoute(nameof(NewCategoryPage), typeof(NewCategoryPage));
            Routing.RegisterRoute(nameof(ScoreRangeDetailPage), typeof(ScoreRangeDetailPage));
            Routing.RegisterRoute(nameof(NewScoreRangePage), typeof(NewScoreRangePage));
            Routing.RegisterRoute(nameof(StatusDetailPage), typeof(StatusDetailPage));
            Routing.RegisterRoute(nameof(NewStatusPage), typeof(NewStatusPage));
            Routing.RegisterRoute(nameof(FilterPage), typeof(FilterPage));
            Routing.RegisterRoute(nameof(LogsPage), typeof(LogsPage));
        }

        private async void MenuItemExport_Clicked(object sender, EventArgs e)
        {
            var logger = provider.GetService<ILogger>();
            try
            {
                string filename = "backup_" + DateTime.UtcNow.ToString("MM-dd-yyyy_HH-mm-ss") + ".bac";
                using var conn = provider.GetService<ISharedDataService>().LoadSave.NewConnection();
                using var stream = new MemoryStream(conn.ExportSaveBackup());
                var result = await provider.GetService<IFileSaver>().SaveAsync(filename, stream, new CancellationToken());
                if (result.IsSuccessful)
                {
                    await provider.GetService<IToastService>().ShowToastAsync("Backup saved: " + result.FilePath);
                    logger.Log("Backup file successfully saved to: " + result.FilePath);
                }
                else
                {
                    logger.Log("Unable to save backup file: " + result.Exception.Message);
                }
            }
            catch (Exception ex)
            {
                logger.Log("Error exporting save backup: " + ex.GetType().Name + " - " + ex.Message);
                logger.Log(ex.StackTrace);
                await provider.GetService<IAlertService>().DisplayAlertAsync("Error", "An error occurred when trying to export the save backup. Please try again later.");
            }
        }

        private async void MenuItemImport_Clicked(object sender, EventArgs e)
        {
            var logger = provider.GetService<ILogger>();
            try
            {
                var result = await provider.GetService<IFilePicker>().PickAsync(new PickOptions()
                {
                    PickerTitle = "Import Save Backup"
                });
                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    stream.CopyTo(ms);

                    using var conn = provider.GetService<ISharedDataService>().LoadSave.NewConnection();
                    conn.ImportSaveBackup(ms.ToArray());

                    await provider.GetService<IAlertService>().DisplayAlertAsync("Import", "Successfully imported the backup save data. Refresh the screen to view the imported data.");
                    logger.Log("Backup file successfully imported from: " + result.FileName);
                    provider.GetService<ISharedDataService>().ResetSharedObjects();
                }
                else
                {
                    logger.Log("Unable to import backup file, operation was canceled");
                }
            }
            catch (Exception ex)
            {
                logger.Log("Error importing save backup: " + ex.GetType().Name + " - " + ex.Message);
                logger.Log(ex.StackTrace);
                await provider.GetService<IAlertService>().DisplayAlertAsync("Error", "An error occurred when trying to import the save backup. Please try again later.");
            }
        }
    }
}