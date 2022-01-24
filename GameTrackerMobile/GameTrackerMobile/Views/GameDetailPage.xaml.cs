using System.ComponentModel;
using GameTrackerMobile.ViewModels;
using Xamarin.Forms;

namespace GameTrackerMobile.Views
{
    public partial class GameDetailPage : ContentPage
    {
        public GameDetailPage()
        {
            InitializeComponent();
            BindingContext = new GameDetailViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}