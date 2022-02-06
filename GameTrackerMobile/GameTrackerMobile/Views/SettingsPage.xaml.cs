using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GameTrackerMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private async void statusButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(StatusPage));
        }

        private async void categoryButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CategoryPage));
        }

        private async void rangesButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ScoreRangePage));
        }
    }
}