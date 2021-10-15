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
using RatableTracker.Framework;

namespace GameTracker.UI
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

        public ListBoxItemScoreRange(ScoreRange sr)
        {
            InitializeComponent();
            this.sr = sr;
            LabelName.Content = sr.Name;
        }
    }
}
