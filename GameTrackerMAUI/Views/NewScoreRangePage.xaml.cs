using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class NewScoreRangePage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public NewScoreRangePage(NewScoreRangeViewModel vm)
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