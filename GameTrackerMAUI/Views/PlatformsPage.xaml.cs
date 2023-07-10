using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class PlatformsPage : ContentPage
{
    PlatformsViewModel _viewModel;

    public PlatformsPage()
	{
		InitializeComponent();
        BindingContext = _viewModel = new PlatformsViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }
}