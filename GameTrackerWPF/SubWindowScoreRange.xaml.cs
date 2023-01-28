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
            LabelError.Visibility = Visibility.Collapsed;
            this.rm = rm;
            this.orig = orig;
            this.settings = settings;
            if (this.orig == null) this.orig = new ScoreRange(rm, settings);
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
                    ComboboxRelationship.SelectedItem = orig.ScoreRelationship;
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
            orig.Name = TextboxName.Text;
            IList<double> valueList = new List<double>();
            IList<string> list = GetValueListText();
            foreach (string str in list)
            {
                if (!double.TryParse(str, out _) || str == "")
                {
                    LabelError.Visibility = Visibility.Visible;
                    LabelError.Content = "All values must be numbers";
                    return;
                }
                valueList.Add(double.Parse(str));
            }
            orig.ValueList = valueList;
            ScoreRelationship sr = (ScoreRelationship)ComboboxRelationship.SelectedItem;
            if (sr == null)
            {
                LabelError.Visibility = Visibility.Visible;
                LabelError.Content = "You must select a score relationship";
                return;
            }
            orig.Color = ColorPickerColor.SelectedColor.ToDrawingColor();
            try
            {
                orig.Save(rm, settings);
            }
            catch (ValidationException e)
            {
                e.DisplayUIExceptionMessage();
                return;
            }
            Close();
        }

        private void FillCombobox()
        {
            ComboboxRelationship.Items.Clear();
            foreach (ScoreRelationship sr in rm.GetScoreRelationshipList())
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

        private void SetValueList(IList<double> valueList)
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

        private IList<string> GetValueListText()
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
