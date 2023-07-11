using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class NewCategoryPage : ContentPage
{
	public NewCategoryPage()
	{
		InitializeComponent();
        BindingContext = new NewCategoryViewModel();
    }
}