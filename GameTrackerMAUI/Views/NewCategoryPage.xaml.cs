using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class NewCategoryPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public NewCategoryPage(NewCategoryViewModel vm)
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