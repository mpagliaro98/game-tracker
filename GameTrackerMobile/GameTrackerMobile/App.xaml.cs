using System;
using GameTrackerMobile.Services;
using GameTrackerMobile.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GameTrackerMobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<GameDataStore>();
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
