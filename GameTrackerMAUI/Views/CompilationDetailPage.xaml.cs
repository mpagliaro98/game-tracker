using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class CompilationDetailPage : ContentPage
{
	public CompilationDetailPage()
	{
		InitializeComponent();
        BindingContext = new CompilationDetailViewModel();
    }
}