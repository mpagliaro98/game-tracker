using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class CategoryPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public CategoryPage(CategoryViewModel vm)
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