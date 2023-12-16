using GameTracker;
using GameTrackerMAUI.Services;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Util;

namespace GameTrackerMAUI
{
    public partial class App : Application
    {
        public App(IServiceProvider provider)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(MauiStartup.GetLicenseKey());
            InitializeComponent();
            MauiExceptions.Init(provider.GetLogger(), provider.GetSavedState(), provider.GetPathController());
            MainPage = new AppShell(provider);
        }
    }
}