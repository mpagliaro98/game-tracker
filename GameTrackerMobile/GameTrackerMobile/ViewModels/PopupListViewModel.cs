﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GameTrackerMobile.ViewModels
{
    public class PopupListViewModel : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public IEnumerable<PopupListOption> Options { get; set; }

        public enum EnumOutputType { Cancel, Selection, Clear }
        public Tuple<EnumOutputType, int?> ReturnValue;

        public Command<PopupListOption> ItemTapped { get; }

        public PopupListViewModel(string title, IEnumerable<PopupListOption> options)
        {
            Title = title;
            Options = options;

            CancelCommand = new Command(async () =>
            {
                await ClosePopUp(EnumOutputType.Cancel, null);
            });
            ClearCommand = new Command(async () =>
            {
                await ClosePopUp(EnumOutputType.Clear, null);
            });

            ItemTapped = new Command<PopupListOption>(OnItemSelected);
        }

        private async Task ClosePopUp(EnumOutputType outputType, int? inputValue)
        {
            ReturnValue = new Tuple<EnumOutputType, int?>(outputType, inputValue);
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CancelCommand { get; protected set; }
        public ICommand ClearCommand { get; protected set; }

        async void OnItemSelected(PopupListOption item)
        {
            await ClosePopUp(EnumOutputType.Selection, item.Value);
        }
    }
}
