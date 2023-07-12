using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class NewScoreRangePage : ContentPage
{
	public NewScoreRangePage()
	{
		InitializeComponent();
        BindingContext = new NewScoreRangeViewModel();
    }
}