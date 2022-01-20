using System.ComponentModel;
using GameTrackerMobile.ViewModels;
using Xamarin.Forms;

namespace GameTrackerMobile.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}