using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using GameTracker;
using GameTrackerMAUI.Services;
using Microsoft.Extensions.Logging;
using RatableTracker.Interfaces;
using RatableTracker.LoadSave;
using RatableTracker.Util;
using SimpleToolkit.Core;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Core.Hosting;

namespace GameTrackerMAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .UseSimpleToolkit()
                .ConfigureSyncfusionCore()
                .RegisterServices()
                .RegisterViewModels()
                .RegisterViews();
#if DEBUG
		    builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            var pathController = new PathControllerMobile();
            builder.Services.AddSingleton<IPathController>(pathController);
            builder.Services.AddSingleton<GameTrackerFactory>();
            builder.Services.AddSingleton<ISavedState, SavedState>();
            builder.Services.AddSingleton<RatableTracker.Interfaces.ILogger>(new LoggerGameTracker(new FileHandlerLocalAppData(pathController, LoggerThreaded.LOG_DIRECTORY)));
            builder.Services.AddSingleton<ISharedDataService, SharedDataService>();
            builder.Services.AddSingleton<IAlertService, AlertServiceMAUI>();
            builder.Services.AddSingleton<IToastService, ToastServiceToolkit>();
            builder.Services.AddSingleton(FileSaver.Default);
            builder.Services.AddSingleton(FilePicker.Default);
            return builder;
        }

        private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ViewModels.GamesViewModel>();
            builder.Services.AddSingleton<ViewModels.PlatformsViewModel>();

            builder.Services.AddTransient<ViewModels.CategoryDetailViewModel>();
            builder.Services.AddTransient<ViewModels.CategoryViewModel>();
            builder.Services.AddTransient<ViewModels.CompilationDetailViewModel>();
            builder.Services.AddTransient<ViewModels.EditCompilationViewModel>();
            builder.Services.AddTransient<ViewModels.FilterViewModel>();
            builder.Services.AddTransient<ViewModels.GameDetailViewModel>();
            builder.Services.AddTransient<ViewModels.NewCategoryViewModel>();
            builder.Services.AddTransient<ViewModels.NewGameViewModel>();
            builder.Services.AddTransient<ViewModels.NewPlatformViewModel>();
            builder.Services.AddTransient<ViewModels.NewScoreRangeViewModel>();
            builder.Services.AddTransient<ViewModels.NewStatusViewModel>();
            builder.Services.AddTransient<ViewModels.PlatformDetailViewModel>();
            builder.Services.AddTransient<ViewModels.ScoreRangeDetailViewModel>();
            builder.Services.AddTransient<ViewModels.ScoreRangeViewModel>();
            builder.Services.AddTransient<ViewModels.SettingsEditViewModel>();
            builder.Services.AddTransient<ViewModels.StatusDetailViewModel>();
            builder.Services.AddTransient<ViewModels.StatusViewModel>();
            builder.Services.AddTransient<ViewModels.LogsViewModel>();
            return builder;
        }

        private static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<Views.GamesPage>();
            builder.Services.AddSingleton<Views.PlatformsPage>();

            builder.Services.AddTransient<Views.AboutPage>();
            builder.Services.AddTransient<Views.CategoryDetailPage>();
            builder.Services.AddTransient<Views.CategoryPage>();
            builder.Services.AddTransient<Views.CompilationDetailPage>();
            builder.Services.AddTransient<Views.EditCompilationPage>();
            builder.Services.AddTransient<Views.FilterPage>();
            builder.Services.AddTransient<Views.GameDetailPage>();
            builder.Services.AddTransient<Views.NewCategoryPage>();
            builder.Services.AddTransient<Views.NewGamePage>();
            builder.Services.AddTransient<Views.NewPlatformPage>();
            builder.Services.AddTransient<Views.NewScoreRangePage>();
            builder.Services.AddTransient<Views.NewStatusPage>();
            builder.Services.AddTransient<Views.PlatformDetailPage>();
            builder.Services.AddTransient<Views.ScoreRangeDetailPage>();
            builder.Services.AddTransient<Views.ScoreRangePage>();
            builder.Services.AddTransient<Views.SettingsEditPage>();
            builder.Services.AddTransient<Views.StatusDetailPage>();
            builder.Services.AddTransient<Views.StatusPage>();
            builder.Services.AddTransient<Views.LogsPage>();
            return builder;
        }
    }
}