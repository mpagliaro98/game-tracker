using System;
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
using RatableTracker.ScoreRanges;
using RatableTracker.Exceptions;
using Xceed.Wpf.Toolkit;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for SubWindowScoreRange.xaml
    /// </summary>
    public partial class SubWindowScoreRange : Window
    {
        private GameModule rm;
        private SettingsGame settings;
        private ScoreRange orig;
        
        public SubWindowScoreRange(GameModule rm, SettingsGame settings, SubWindowMode mode, ScoreRange orig)
        {
            InitializeComponent();
            this.rm = rm;
            this.orig = orig;
            this.settings = settings;

            // initialize UI containers
            FillCombobox();
            ButtonSave.Content = mode == SubWindowMode.MODE_ADD ? "Create" : "Update";

            // set fields in the UI
            TextboxName.Text = orig.Name;
            ColorPickerColor.SelectedColor = orig.Color.ToMediaColor();
            if (orig.ScoreRelationship != null) ComboboxRelationship.SelectedItem = orig.ScoreRelationship;

            // set event handlers
            TextboxName.TextChanged += TextboxName_TextChanged;
            ComboboxRelationship.SelectionChanged += ComboboxRelationship_SelectionChanged;
            ColorPickerColor.SelectedColorChanged += ColorPickerColor_SelectedColorChanged;

            // refresh UI logic
            RefreshValueList();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                orig.Save(rm, settings);
            }
            catch (ValidationException ex)
            {
                ex.DisplayUIExceptionMessage();
                return;
            }
            Close();
        }

        private void FillCombobox()
        {
            ComboboxRelationship.Items.Clear();
            var item = new ComboBoxItem();
            item.Content = "N/A";
            ComboboxRelationship.Items.Add(item);
            foreach (ScoreRelationship sr in rm.GetScoreRelationshipList().OrderBy(p => p.Name))
            {
                ComboboxRelationship.Items.Add(sr);
            }
            ComboboxRelationship.SelectedIndex = 0;
        }

        private void RefreshValueList()
        {
            StackPanelValueList.Children.Clear();
            int count = ComboboxRelationship.SelectedIndex > 0 ? ((ScoreRelationship)ComboboxRelationship.SelectedItem).NumValuesRequired : 1;
            for (int i = 0; i < count; i++)
            {
                DoubleUpDown tb = new()
                {
                    Margin = new Thickness { Left = 5 },
                    Width = 100,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Value = orig.ValueList[i],
                    Tag = i
                };
                tb.ValueChanged += Tb_ValueChanged;
                StackPanelValueList.Children.Add(tb);
            }
        }

        private void TextboxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            orig.Name = TextboxName.Text.Trim();
        }

        private void ComboboxRelationship_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            orig.ScoreRelationship = ComboboxRelationship.SelectedIndex > 0 ? (ScoreRelationship)ComboboxRelationship.SelectedItem : null;
            orig.ValueList = new List<double>();
            for (int i = 0; i < (ComboboxRelationship.SelectedIndex > 0 ? orig.ScoreRelationship.NumValuesRequired : 1); i++)
            {
                orig.ValueList.Add(settings.MinScore);
            }
            RefreshValueList();
        }

        private void ColorPickerColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            orig.Color = ColorPickerColor.SelectedColor.ToDrawingColor();
        }

        private void Tb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DoubleUpDown box = (DoubleUpDown)sender;
            int index = (int)box.Tag;
            orig.ValueList[index] = box.Value ?? settings.MinScore;
        }
    }
}
