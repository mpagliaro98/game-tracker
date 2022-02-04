using System;
using System.Collections.Generic;
using GameTrackerMobile.ViewModels;
using GameTrackerMobile.Views;
using Xamarin.Forms;

namespace GameTrackerMobile
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(GameDetailPage), typeof(GameDetailPage));
            Routing.RegisterRoute(nameof(NewGamePage), typeof(NewGamePage));
            Routing.RegisterRoute(nameof(PlatformDetailPage), typeof(PlatformDetailPage));
            Routing.RegisterRoute(nameof(NewPlatformPage), typeof(NewPlatformPage));
            Routing.RegisterRoute(nameof(StatusPage), typeof(StatusPage));
            Routing.RegisterRoute(nameof(StatusDetailPage), typeof(StatusDetailPage));
            Routing.RegisterRoute(nameof(NewStatusPage), typeof(NewStatusPage));
        }

    }
}
