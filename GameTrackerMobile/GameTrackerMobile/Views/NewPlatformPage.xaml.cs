﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTrackerMobile.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace GameTrackerMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPlatformPage : ContentPage
    {
        public NewPlatformPage()
        {
            InitializeComponent();
            BindingContext = new NewPlatformViewModel();
        }
    }
}