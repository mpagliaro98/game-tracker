﻿using GameTrackerMAUI.Views;

namespace GameTrackerMAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(NewGamePage), typeof(NewGamePage));
            Routing.RegisterRoute(nameof(GameDetailPage), typeof(GameDetailPage));
            Routing.RegisterRoute(nameof(NewPlatformPage), typeof(NewPlatformPage));
            Routing.RegisterRoute(nameof(PlatformDetailPage), typeof(PlatformDetailPage));
            Routing.RegisterRoute(nameof(CompilationDetailPage), typeof(CompilationDetailPage));
            Routing.RegisterRoute(nameof(EditCompilationPage), typeof(EditCompilationPage));
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
            Routing.RegisterRoute(nameof(SettingsEditPage), typeof(SettingsEditPage));
            Routing.RegisterRoute(nameof(CategoryPage), typeof(CategoryPage));
            Routing.RegisterRoute(nameof(CategoryDetailPage), typeof(CategoryDetailPage));
            Routing.RegisterRoute(nameof(NewCategoryPage), typeof(NewCategoryPage));
        }
    }
}