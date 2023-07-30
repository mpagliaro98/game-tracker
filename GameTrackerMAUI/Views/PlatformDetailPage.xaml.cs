using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class PlatformDetailPage : ContentPage
{
    BaseViewModel _viewModel;

    public PlatformDetailPage(PlatformDetailViewModel vm)
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