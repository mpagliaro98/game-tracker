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

namespace GameTracker.UI
{
    /// <summary>
    /// Interaction logic for ScalableListBox.xaml
    /// </summary>
    public partial class ScalableListBox : UserControl
    {
        public ScalableListBox()
        {
            InitializeComponent();
        }

        public void ClearItems()
        {
            LB.Items.Clear();
        }

        public void AddItem(object c)
        {
            LB.Items.Add(c);
        }

        public void RemoveItem(object c)
        {
            LB.Items.Remove(c);
        }
    }
}
