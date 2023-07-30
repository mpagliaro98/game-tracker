using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class SettingsEditPage : ContentPage
{
    BaseViewModel _viewModel;

    public SettingsEditPage(SettingsEditViewModel vm)
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