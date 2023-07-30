using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class EditCompilationPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public EditCompilationPage(EditCompilationViewModel vm)
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