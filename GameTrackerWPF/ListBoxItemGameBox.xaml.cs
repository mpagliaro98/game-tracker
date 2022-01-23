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
using GameTracker.Model;
using RatableTracker.Framework.Global;

namespace GameTrackerWPF
{
    /// <summary>
    /// Interaction logic for ListBoxItemGameBox.xaml
    /// </summary>
    public partial class ListBoxItemGameBox : UserControl, IListBoxItemGame
    {
        private const string DECIMAL_FORMAT = "0.##";

        private RatableGame rg;
        public RatableGame Game
        {
            get { return rg; }
        }

        public ListBoxItemGameBox(RatingModuleGame rm, RatableGame rg)
        {
            InitializeComponent();
            this.rg = rg;

            var platform = rm.FindPlatform(rg.RefPlatform);
            var completionStatus = rm.FindStatus(rg.RefStatus);

            TextBlockName.Text = rg.Name;
            TextBlockPlatform.Text = platform != null ? platform.Name : "";
            TextBlockStatus.Text = completionStatus != null ? completionStatus.Name : "";
            if (completionStatus != null)
                TextBlockStatus.Background = new SolidColorBrush(completionStatus.Color.ToMediaColor());
            TextBlockFinalScore.Text = rm.GetScoreOfObject(rg).ToString(DECIMAL_FORMAT);
            TextBlockFinalScore.Background = new SolidColorBrush(rm.GetRangeColorFromObject(rg).ToMediaColor());
        }
    }
}
