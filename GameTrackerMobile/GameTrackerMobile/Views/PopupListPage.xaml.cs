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
    public partial class PopupListPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        private TaskCompletionSource<Tuple<PopupListViewModel.EnumOutputType, int?>> _taskCompletionSource;
        public Task<Tuple<PopupListViewModel.EnumOutputType, int?>> PopupClosedTask => _taskCompletionSource.Task;


        public PopupListPage(string title, IEnumerable<PopupListOption> options)
        {
            InitializeComponent();
            BindingContext = new PopupListViewModel(title, options);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _taskCompletionSource = new TaskCompletionSource<Tuple<PopupListViewModel.EnumOutputType, int?>>();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _taskCompletionSource.SetResult(((PopupListViewModel)BindingContext).ReturnValue);
        }
    }
}