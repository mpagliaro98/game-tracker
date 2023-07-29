using System.Reflection;

namespace GameTrackerMAUI.Views;

public partial class AboutPage : ContentPage
{
	public AboutPage()
	{
		InitializeComponent();
        aboutLabel.Text = "Game Tracker: " + Assembly.GetExecutingAssembly().GetName().Version.ToString() +
                "\nAuthor: Michael Pagliaro" +
                "\nGitHub: github.com/mpagliaro98" +
                "\nThis open-source software is covered under the MIT license, see the license in the GitHub repository for more information.";
    }
}