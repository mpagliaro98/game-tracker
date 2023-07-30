using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class CategoryDetailPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public CategoryDetailPage(CategoryDetailViewModel vm)
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