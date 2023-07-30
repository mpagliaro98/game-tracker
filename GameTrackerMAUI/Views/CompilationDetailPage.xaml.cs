using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class CompilationDetailPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public CompilationDetailPage(CompilationDetailViewModel vm)
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