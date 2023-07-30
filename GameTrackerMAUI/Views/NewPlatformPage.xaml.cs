using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class NewPlatformPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public NewPlatformPage(NewPlatformViewModel vm)
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