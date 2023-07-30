using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class StatusPage : ContentPage
{
    BaseViewModel _viewModel;

    public StatusPage(StatusViewModel vm)
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