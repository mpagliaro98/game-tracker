using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class NewPlatformPage : ContentPage
{
	public NewPlatformPage()
	{
		InitializeComponent();
		BindingContext = new NewPlatformViewModel();
	}
}