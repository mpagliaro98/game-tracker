using GameTrackerMAUI.Views;

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
            Routing.RegisterRoute(nameof(CategoryDetailPage), typeof(CategoryDetailPage));
            Routing.RegisterRoute(nameof(NewCategoryPage), typeof(NewCategoryPage));
            Routing.RegisterRoute(nameof(ScoreRangeDetailPage), typeof(ScoreRangeDetailPage));
            Routing.RegisterRoute(nameof(NewScoreRangePage), typeof(NewScoreRangePage));
            Routing.RegisterRoute(nameof(StatusDetailPage), typeof(StatusDetailPage));
            Routing.RegisterRoute(nameof(NewStatusPage), typeof(NewStatusPage));
            Routing.RegisterRoute(nameof(FilterPage), typeof(FilterPage));
        }
    }
}