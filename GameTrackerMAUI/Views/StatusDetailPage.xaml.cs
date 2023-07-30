using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class StatusDetailPage : ContentPage
{
    BaseViewModel _viewModel;

    public StatusDetailPage(StatusDetailViewModel vm)
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