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
    public partial class PlatformDetailPage : ContentPage
    {
        public PlatformDetailPage()
        {
            InitializeComponent();
            BindingContext = new PlatformDetailViewModel();
        }
    }
}