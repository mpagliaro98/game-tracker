using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class GameDetailPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public GameDetailPage(GameDetailViewModel vm)
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