using GameTrackerMobile.ViewModels;
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
    public partial class StatusDetailPage : ContentPage
    {
        public StatusDetailPage()
        {
            InitializeComponent();
            BindingContext = new StatusDetailViewModel();
        }
    }
}