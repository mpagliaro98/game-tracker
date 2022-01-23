using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTrackerMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GameTrackerMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamesPage : ContentPage
    {
        GamesViewModel _viewModel;

        public GamesPage()
        {
            InitializeComponent();
            
            BindingContext = _viewModel = new GamesViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}