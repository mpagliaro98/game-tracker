using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class StatusPage : ContentPage
{
    StatusViewModel _viewModel;

    public StatusPage()
    {
        InitializeComponent();
        BindingContext = _viewModel = new StatusViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }
}