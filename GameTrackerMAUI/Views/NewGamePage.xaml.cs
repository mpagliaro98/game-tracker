using GameTrackerMAUI.ViewModels;
using SimpleToolkit.Core;
using Syncfusion.Maui.Picker;
using System.DirectoryServices;

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

    private void slider_DragStarted(object sender, EventArgs e)
    {
        Slider slider = (Slider)sender;
        slider.ShowAttachedPopover();
    }

    private void slider_DragCompleted(object sender, EventArgs e)
    {
        Slider slider = (Slider)sender;
        slider.HideAttachedPopover();
    }

    private void BAcquired_Clicked(object sender, EventArgs e)
    {
        DPAcquired.IsOpen = true;
    }

    private void BRelease_Clicked(object sender, EventArgs e)
    {
        DPRelease.IsOpen = true;
    }

    private void BStart_Clicked(object sender, EventArgs e)
    {
        DPStart.IsOpen = true;
    }

    private void BFinish_Clicked(object sender, EventArgs e)
    {
        DPFinish.IsOpen = true;
    }

    private void DatePickerOpened(object sender, EventArgs e)
    {
        var picker = (SfDatePicker)sender;
        if (picker.SelectedDate <= picker.MinimumDate)
            picker.SelectedDate = DateTime.Today;
    }
}