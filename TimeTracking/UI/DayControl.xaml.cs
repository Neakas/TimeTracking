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
using TimeTracking.Database;

namespace TimeTracking.UI
{
    /// <summary>
    /// Interaction logic for DayControl.xaml
    /// </summary>
    public partial class DayControl : UserControl
    {
        private TTDates _day;
        public DayControl(TTDates day)
        {
            InitializeComponent();
            //_day = day;
            //day.TTEntry.ToList().ForEach(x =>
            //{
            //    MainStackpanel.Children.Add(new DayEntry(x));
            //});
        }

        private void btNewEntry_Click(object sender, RoutedEventArgs e)
        {
            //DBManager.CreateEntry(_day);
        }
    }
}
