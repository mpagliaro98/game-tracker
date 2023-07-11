using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class CategoryDetailPage : ContentPage
{
	public CategoryDetailPage()
	{
		InitializeComponent();
        BindingContext = new CategoryDetailViewModel();
    }
}