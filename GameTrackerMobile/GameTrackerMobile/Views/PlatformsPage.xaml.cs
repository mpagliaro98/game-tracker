using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTrackerMobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GameTrackerMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlatformsPage : ContentPage
    {
        PlatformsViewModel _viewModel;

        public PlatformsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new PlatformsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}