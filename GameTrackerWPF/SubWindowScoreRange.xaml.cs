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
using RatableTracker.Framework;
using GameTracker.Model;
using RatableTracker.Framework.ScoreRelationships;
using RatableTracker.Framework.Global;
using RatableTracker.Framework.Exceptions;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for SubWindowScoreRange.xaml
    /// </summary>
    public partial class SubWindowScoreRange : Window
    {
        private RatingModuleGame rm;
        private ScoreRange orig;
        
        public SubWindowScoreRange(RatingModuleGame rm, SubWindowMode mode, ScoreRange orig = null)
        {
            InitializeComponent();
            LabelError.Visibility = Visibility.Collapsed;
            this.rm = rm;
            this.orig = orig;
            FillCombobox();
            switch (mode)
            {
                case SubWindowMode.MODE_ADD:
                    ButtonSave.Visibility = Visibility.Visible;
                    ButtonUpdate.Visibility = Visibility.Collapsed;
                    ComboboxRelationship.SelectedIndex = 0;
                    ResetValueList();
                    break;
                case SubWindowMode.MODE_EDIT:
                    ButtonSave.Visibility = Visibility.Collapsed;
                    ButtonUpdate.Visibility = Visibility.Visible;
                    TextboxName.Text = orig.Name;
                    ComboboxRelationship.SelectedItem = rm.FindScoreRelationship(orig.RefScoreRelationship);
                    ColorPickerColor.SelectedColor = orig.Color.ToMediaColor();
                    SetValueList(orig.ValueList);
                    break;
                default:
                    throw new Exception("Unhandled mode");
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveResult();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            SaveResult();
        }

        private void SaveResult()
        {
            if (!ValidateInputs(out string name, out IEnumerable<double> valueList,
                out ScoreRelationship sr, out RatableTracker.Framework.Color color)) return;
            var range = new ScoreRange()
            {
                Name = name,
                ValueList = valueList,
                Color = color
            };
            range.SetScoreRelationship(sr);
            try
            {
                if (orig == null)
                    rm.AddRange(range);
                else
                    rm.UpdateRange(range, orig);
            }
            catch (ValidationException e)
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = e.Message;
                return;
            }
            Close();
        }

        private bool ValidateInputs(out string name, out IEnumerable<double> valueList, out ScoreRelationship sr,
            out RatableTracker.Framework.Color color)
        {
            name = TextboxName.Text;
            valueList = new List<double>();
            IEnumerable<string> list = GetValueListText();
            sr = (ScoreRelationship)ComboboxRelationship.SelectedItem;
            color = ColorPickerColor.SelectedColor.ToDrawingColor();
            if (sr == null)
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = "You must select a score relationship";
                return false;
            }
            foreach (string str in list)
            {
                if (!double.TryParse(str, out _) || str == "")
                {
                    LabelError.Visibility = Visibility.Visible;
                    LabelError.Content = "All values must be numbers";
                    return false;
                }
                valueList = valueList.Append(double.Parse(str));
            }
            return true;
        }

        private void FillCombobox()
        {
            ComboboxRelationship.Items.Clear();
            foreach (ScoreRelationship sr in rm.ScoreRelationships)
            {
                ComboboxRelationship.Items.Add(sr);
            }
        }

        private void RefreshValueList()
        {
            int currentCount = StackPanelValueList.Children.Count;
            int requiredCount = ((ScoreRelationship)ComboboxRelationship.SelectedItem).NumValuesRequired;
            while (currentCount != requiredCount)
            {
                if (currentCount < requiredCount)
                {
                    TextBox tb = new TextBox
                    {
                        Margin = new Thickness { Left = 5 },
                        Width = 100,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    StackPanelValueList.Children.Add(tb);
                    currentCount += 1;
                }
                else
                {
                    StackPanelValueList.Children.RemoveAt(currentCount - 1);
                    currentCount -= 1;
                }
            }
        }

        private void ResetValueList()
        {
            RefreshValueList();
            foreach (TextBox tb in StackPanelValueList.Children)
            {
                tb.Text = "";
            }
        }

        private void SetValueList(IEnumerable<double> valueList)
        {
            RefreshValueList();
            if (valueList == null) return;
            List<double> list = valueList.ToList();
            for (int i = 0; i < valueList.Count(); i += 1)
            {
                TextBox tb = (TextBox)StackPanelValueList.Children[i];
                tb.Text = list[i].ToString();
            }
        }

        private IEnumerable<string> GetValueListText()
        {
            List<string> list = new List<string>();
            foreach (TextBox tb in StackPanelValueList.Children)
            {
                list.Add(tb.Text);
            }
            return list;
        }

        private void ComboboxRelationship_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshValueList();
        }
    }
}
