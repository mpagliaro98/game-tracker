using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class CategoryPage : ContentPage
{
    CategoryViewModel _viewModel;

    public CategoryPage()
    {
        InitializeComponent();
        BindingContext = _viewModel = new CategoryViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }
}