using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class ScoreRangePage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public ScoreRangePage(ScoreRangeViewModel vm)
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