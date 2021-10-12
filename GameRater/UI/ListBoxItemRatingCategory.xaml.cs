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
using GameRater.Model;

namespace GameRater.UI
{
    /// <summary>
    /// Interaction logic for ListBoxItemRatingCategory.xaml
    /// </summary>
    public partial class ListBoxItemRatingCategory : UserControl
    {
        public ListBoxItemRatingCategory()
        {
            InitializeComponent();
        }

        public void SetContent(RatingCategory rc)
        {
            LabelName.Content = rc.Name;
            LabelWeight.Content = "Weight: " + rc.GetWeight().ToString();
        }
    }
}
