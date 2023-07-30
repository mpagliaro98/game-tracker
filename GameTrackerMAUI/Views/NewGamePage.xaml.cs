using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class NewGamePage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public NewGamePage(NewGameViewModel vm)
	{
		InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }

    private void DatePicker_Focused(object sender, FocusEventArgs e)
    {
        var picker = (DatePicker)sender;
        if (picker.Date <= picker.MinimumDate)
            picker.Date = DateTime.Today;
    }
}