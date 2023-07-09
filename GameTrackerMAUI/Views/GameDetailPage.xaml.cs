using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class GameDetailPage : ContentPage
{
	public GameDetailPage()
	{
		InitializeComponent();
		BindingContext = new GameDetailViewModel();
	}
}