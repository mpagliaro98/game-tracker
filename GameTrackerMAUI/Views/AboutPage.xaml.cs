using System.Reflection;

namespace GameTrackerMAUI.Views;

public partial class AboutPage : ContentPage
{
	public AboutPage()
	{
		InitializeComponent();
        aboutLabel.Text = "Game Tracker: " + Assembly.GetExecutingAssembly().GetName().Version.ToString() +
#if DEBUG
                " DEBUG VERSION" +
#endif
                "\nAuthor: Michael Pagliaro" +
                "\nGitHub: github.com/mpagliaro98" +
                "\nThis open-source software is covered under the MIT license, see the license in the GitHub repository for more information.";
    }

    private async void ButtonRepo_Clicked(object sender, EventArgs e)
    {
        try
        {
            Uri uri = new Uri("https://github.com/mpagliaro98/game-tracker");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch
        {
            // An unexpected error occurred. No browser may be installed on the device.
        }
    }

    private async void ButtonUpdates_Clicked(object sender, EventArgs e)
    {
        try
        {
            Uri uri = new Uri("https://github.com/mpagliaro98/game-tracker/releases");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch
        {
            // An unexpected error occurred. No browser may be installed on the device.
        }
    }
}