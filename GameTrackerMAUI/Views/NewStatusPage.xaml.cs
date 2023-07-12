using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class NewStatusPage : ContentPage
{
	public NewStatusPage()
	{
		InitializeComponent();
        BindingContext = new NewStatusViewModel();
    }
}