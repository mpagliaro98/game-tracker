namespace GameTrackerMAUI.Views;

public partial class AboutPage : ContentPage
{
	public AboutPage()
	{
		InitializeComponent();
        aboutLabel.Text = "Game Tracker: " + GameTracker.Util.GameTrackerVersion.ToString() +
                "\nFramework: " + RatableTracker.Util.Util.FrameworkVersion.ToString() +
                "\nAuthor: Michael Pagliaro" +
                "\nGitHub: github.com/mpagliaro98" +
                "\nThis open-source software is covered under the MIT license, see the license in the GitHub repository for more information.";
    }
}