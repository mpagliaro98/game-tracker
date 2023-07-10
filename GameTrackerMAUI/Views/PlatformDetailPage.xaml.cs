using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class PlatformDetailPage : ContentPage
{
	public PlatformDetailPage()
	{
		InitializeComponent();
		BindingContext = new PlatformDetailViewModel();
	}
}