using GameTracker;
using RatableTracker.ScoreRanges;
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
    /// Interaction logic for ListBoxItemScoreRange.xaml
    /// </summary>
    public partial class ListBoxItemScoreRange : UserControl
    {
        private ScoreRange sr;
        public ScoreRange ScoreRange
        {
            get { return sr; }
        }

        public ListBoxItemScoreRange(GameModule rm, ScoreRange sr)
        {
            InitializeComponent();
            this.sr = sr;
            LabelName.Content = sr.Name;
            LabelRelationship.Content = sr.ScoreRelationship.Name + " " + GetRelationshipValues(sr.ValueList);
            RectangeColor.Fill = new SolidColorBrush(sr.Color.ToMediaColor());
        }

        private string GetRelationshipValues(IEnumerable<double> valueList)
        {
            List<double> list = valueList.ToList();
            string result = "";
            for (int i = 0; i < valueList.Count(); i += 1)
            {
                if (result != "" && valueList.Count() > 2) result += ", ";
                if (result != "" && i == valueList.Count() - 1) result += " and ";
                result += list[i].ToString();
            }
            return result;
        }
    }
}
