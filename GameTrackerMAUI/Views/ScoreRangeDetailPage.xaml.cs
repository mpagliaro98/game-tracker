using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class ScoreRangeDetailPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public ScoreRangeDetailPage(ScoreRangeDetailViewModel vm)
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