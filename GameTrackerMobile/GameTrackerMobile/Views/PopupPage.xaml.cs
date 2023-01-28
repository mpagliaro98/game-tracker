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
    public partial class PopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        private TaskCompletionSource<Tuple<PopupViewModel.EnumOutputType, string>> _taskCompletionSource;
        public Task<Tuple<PopupViewModel.EnumOutputType, string>> PopupClosedTask => _taskCompletionSource.Task;


        public PopupPage(string title, string message, PopupViewModel.EnumInputType inputType)
        {
            InitializeComponent();
            BindingContext = new PopupViewModel(title, message, inputType);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _taskCompletionSource = new TaskCompletionSource<Tuple<PopupViewModel.EnumOutputType, string>>();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _taskCompletionSource.SetResult(((PopupViewModel)BindingContext).ReturnValue);
        }
    }
}