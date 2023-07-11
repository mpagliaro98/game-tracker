using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class EditCompilationPage : ContentPage
{
	public EditCompilationPage()
	{
		InitializeComponent();
        BindingContext = new EditCompilationViewModel();
    }
}