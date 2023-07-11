using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class SettingsEditPage : ContentPage
{
	public SettingsEditPage()
	{
		InitializeComponent();
        BindingContext = new SettingsEditViewModel();
    }
}