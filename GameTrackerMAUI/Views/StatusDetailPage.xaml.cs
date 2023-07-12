using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class StatusDetailPage : ContentPage
{
	public StatusDetailPage()
	{
		InitializeComponent();
        BindingContext = new StatusDetailViewModel();
    }
}