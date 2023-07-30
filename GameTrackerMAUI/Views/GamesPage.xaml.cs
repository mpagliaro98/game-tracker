using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class GamesPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public GamesPage(GamesViewModel vm)
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