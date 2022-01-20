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
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}
