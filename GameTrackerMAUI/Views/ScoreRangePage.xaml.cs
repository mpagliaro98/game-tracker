using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class ScoreRangePage : ContentPage
{
    ScoreRangeViewModel _viewModel;

    public ScoreRangePage()
    {
        InitializeComponent();
        BindingContext = _viewModel = new ScoreRangeViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }
}