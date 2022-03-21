using System;
using GameTrackerMobile.Services;
using GameTrackerMobile.Views;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GameTrackerMobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            PathController.PathControllerInstance = new PathControllerMobile();
            GlobalSettings.Autosave = true;
            SavedState.LoadSavedState();

            DependencyService.Register<GameDataStore>();
            DependencyService.Register<PlatformDataStore>();
            DependencyService.Register<StatusDataStore>();
            DependencyService.Register<CategoryDataStore>();
            DependencyService.Register<ScoreRangeDataStore>();
            DependencyService.Register<CompilationDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
