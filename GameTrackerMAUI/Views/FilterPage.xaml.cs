using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class FilterPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public FilterPage(FilterViewModel vm)
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