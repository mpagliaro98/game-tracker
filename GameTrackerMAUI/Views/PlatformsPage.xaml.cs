using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class PlatformsPage : ContentPage
{
    BaseViewModel _viewModel;

    public PlatformsPage(PlatformsViewModel vm)
	{
		InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }
}