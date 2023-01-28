using GameTrackerMobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace GameTrackerMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScoreRangeDetailPage : ContentPage
    {
        public ScoreRangeDetailPage()
        {
            InitializeComponent();
            BindingContext = new ScoreRangeDetailViewModel();
        }
    }
}