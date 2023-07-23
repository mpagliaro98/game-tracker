using RatableTracker.ListManipulation.Filtering;
using RatableTracker.Util;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for FilterOptionList.xaml
    /// </summary>
    public partial class FilterOptionList : UserControl, IFilterOptionControl
    {
        public object FilterValues => ((KeyValuePair<UniqueID, string>)ComboBoxList.SelectedItem).Key.ToString();

        public FilterOptionList(IFilterOptionList option, object initialValue)
        {
            InitializeComponent();
            FillComboBox(option);
            if (initialValue != null) SetInitialValues((string)initialValue);
        }

        private void FillComboBox(IFilterOptionList option)
        {
            ComboBoxList.Items.Clear();
            foreach (object listItem in option.ListValues)
            {
                ComboBoxList.Items.Add(listItem);
            }
            ComboBoxList.SelectedIndex = 0;
        }

        private void SetInitialValues(string value)
        {
            var uniqueID = UniqueID.Parse(value);
            ComboBoxList.SelectedValue = uniqueID;
            if (ComboBoxList.SelectedValue == null) ComboBoxList.SelectedIndex = 0;
        }
    }
}
