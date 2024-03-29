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
using GameTracker;
using MahApps.Metro.Controls;
using RatableTracker.Exceptions;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for SubWindowCompletionStatus.xaml
    /// </summary>
    public partial class SubWindowCompletionStatus : MetroWindow
    {
        private GameModule rm;
        private StatusGame orig;
        private SettingsGame settings;

        public event EventHandler Saved;

        public SubWindowCompletionStatus(GameModule rm, SettingsGame settings, SubWindowMode mode, StatusGame orig)
        {
            InitializeComponent();
            this.rm = rm;
            this.orig = orig;
            this.settings = settings;

            // initialize UI containers
            FillCombobox();
            ButtonSave.Content = mode == SubWindowMode.MODE_ADD ? "Create" : "Update";

            // set max length
            TextboxName.MaxLength = StatusGame.MaxLengthName;

            // set fields in the UI
            TextboxName.Text = orig.Name;
            ColorPickerColor.SelectedColor = orig.Color.ToMediaColor();
            CheckboxUseAsFinished.IsChecked = orig.UseAsFinished;
            CheckboxUseAsFinished.Visibility = orig.StatusUsage == StatusUsage.UnfinishableGamesOnly ? Visibility.Collapsed : Visibility.Visible;
            CheckboxHideScore.IsChecked = orig.HideScoreFromList;
            ComboboxUsage.SelectedValue = orig.StatusUsage;

            // set event handlers
            TextboxName.TextChanged += TextboxName_TextChanged;
            ColorPickerColor.SelectedColorChanged += ColorPickerColor_SelectedColorChanged;
            CheckboxUseAsFinished.Checked += CheckboxUseAsFinished_Checked;
            CheckboxUseAsFinished.Unchecked += CheckboxUseAsFinished_Checked;
            CheckboxHideScore.Checked += CheckboxHideScore_Checked;
            CheckboxHideScore.Unchecked += CheckboxHideScore_Checked;
            ComboboxUsage.SelectionChanged += ComboboxUsage_SelectionChanged;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                orig.Save(rm, settings);
            }
            catch (Exception ex)
            {
                ex.DisplayUIExceptionMessage();
                return;
            }
            Saved?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void FillCombobox()
        {
            ComboboxUsage.Items.Clear();
            ComboboxUsage.SelectedValuePath = "Key";
            ComboboxUsage.DisplayMemberPath = "Value";
            foreach (var usage in Enum.GetValues<StatusUsage>())
            {
                ComboboxUsage.Items.Add(new KeyValuePair<StatusUsage, string>(usage, usage.StatusUsageToString()));
            }
            ComboboxUsage.SelectedValue = StatusUsage.AllGames;
        }

        private void TextboxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.Name = TextboxName.Text.Trim();
        }

        private void ColorPickerColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            orig.Color = ColorPickerColor.SelectedColor.ToDrawingColor();
        }

        private void CheckboxUseAsFinished_Checked(object sender, RoutedEventArgs e)
        {
            orig.UseAsFinished = CheckboxUseAsFinished.IsChecked.Value;
        }

        private void CheckboxHideScore_Checked(object sender, RoutedEventArgs e)
        {
            orig.HideScoreFromList = CheckboxHideScore.IsChecked.Value;
        }

        private void ComboboxUsage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            orig.StatusUsage = (StatusUsage)ComboboxUsage.SelectedValue;
            CheckboxUseAsFinished.Visibility = orig.StatusUsage == StatusUsage.UnfinishableGamesOnly ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
