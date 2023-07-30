using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class LogsPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public LogsPage(LogsViewModel vm)
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