using System;
using System.Collections.Generic;
using System.ComponentModel;
using GameTracker.Model;
using GameTrackerMobile.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace GameTrackerMobile.Views
{
    public partial class NewGamePage : ContentPage
    {
        public NewGamePage()
        {
            InitializeComponent();
            BindingContext = new NewGameViewModel();
        }

        private void DatePicker_Focused(object sender, FocusEventArgs e)
        {
            var picker = (DatePicker)sender;
            if (picker.Date <= picker.MinimumDate)
                picker.Date = DateTime.Today;
        }
    }
}