using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class FilterPage : ContentPage
{
	public FilterPage()
	{
		InitializeComponent();
        BindingContext = new FilterViewModel();
    }
}