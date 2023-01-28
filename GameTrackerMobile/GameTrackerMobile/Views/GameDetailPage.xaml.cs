using System.ComponentModel;
using GameTrackerMobile.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace GameTrackerMobile.Views
{
    public partial class GameDetailPage : ContentPage
    {
        public GameDetailPage()
        {
            InitializeComponent();
            BindingContext = new GameDetailViewModel();
        }
    }
}