using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class ScoreRangeDetailPage : ContentPage
{
	public ScoreRangeDetailPage()
	{
		InitializeComponent();
        BindingContext = new ScoreRangeDetailViewModel();
    }
}