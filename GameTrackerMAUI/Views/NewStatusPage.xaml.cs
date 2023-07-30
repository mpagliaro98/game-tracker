using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class NewStatusPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public NewStatusPage(NewStatusViewModel vm)
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