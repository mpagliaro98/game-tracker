﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GameTracker.Model;
using RatableTracker.Framework.Global;

namespace GameTracker.UI
{
    /// <summary>
    /// Interaction logic for SubWindowPlatform.xaml
    /// </summary>
    public partial class SubWindowPlatform : Window
    {
        private RatingModuleGame rm;
        private Platform orig;

        public SubWindowPlatform(RatingModuleGame rm, SubWindowMode mode, Platform orig = null)
        {
            InitializeComponent();
            LabelError.Visibility = Visibility.Collapsed;
            this.rm = rm;
            this.orig = orig;
            switch (mode)
            {
                case SubWindowMode.MODE_ADD:
                    ButtonSave.Visibility = Visibility.Visible;
                    ButtonUpdate.Visibility = Visibility.Collapsed;
                    break;
                case SubWindowMode.MODE_EDIT:
                    ButtonSave.Visibility = Visibility.Collapsed;
                    ButtonUpdate.Visibility = Visibility.Visible;
                    TextboxName.Text = orig.Name;
                    TextboxYear.Text = orig.ReleaseYear > 0 ? orig.ReleaseYear.ToString() : "";
                    TextboxAcquiredYear.Text = orig.AcquiredYear > 0 ? orig.AcquiredYear.ToString() : "";
                    ColorPickerColor.SelectedColor = orig.Color.ToMediaColor();
                    break;
                default:
                    throw new Exception("Unhandled mode");
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out string name, out System.Drawing.Color color,
                out int releaseYear, out int acquiredYear)) return;
            var platform = new Platform()
            {
                Name = name,
                Color = color,
                ReleaseYear = releaseYear,
                AcquiredYear = acquiredYear
            };
            rm.AddPlatform(platform);
            Close();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out string name, out System.Drawing.Color color,
                out int releaseYear, out int acquiredYear)) return;
            orig.Name = name;
            orig.Color = color;
            orig.ReleaseYear = releaseYear;
            orig.AcquiredYear = acquiredYear;
            rm.SavePlatforms();
            Close();
        }

        private bool ValidateInputs(out string name, out System.Drawing.Color color,
            out int releaseYear, out int acquiredYear)
        {
            name = TextboxName.Text;
            releaseYear = TextboxYear.Text == "" ? 0 : TextboxYear.Value.HasValue ? TextboxYear.Value.Value : 0;
            acquiredYear = TextboxAcquiredYear.Text == "" ? 0 : TextboxAcquiredYear.Value.HasValue ? TextboxAcquiredYear.Value.Value : 0;
            color = ColorPickerColor.SelectedColor.ToDrawingColor();
            if (name == "")
            {
                LabelError.Visibility = Visibility.Visible;
                return false;
            }
            return true;
        }
    }
}
