namespace GameTrackerMAUI.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

    private async void statusButton_Clicked(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync(nameof(StatusPage));
    }

    private async void categoryButton_Clicked(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync(nameof(CategoryPage));
    }

    private async void rangesButton_Clicked(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync(nameof(ScoreRangePage));
    }

    private async void generalSettingsButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsEditPage));
    }

    private async void aboutButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AboutPage));
    }
}