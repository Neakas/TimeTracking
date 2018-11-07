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
    /// Interaction logic for DayEntry.xaml
    /// </summary>
    public partial class DayEntry
    {
        //private TTEntry _entry;
        //public DayEntry(TTEntry entry)
        //{
        //    InitializeComponent();
        //    _entry = entry;
        //    MainExpander.Header = entry.Header;
        //    MainTextBlock.Text = entry.Entry;
        //    MainTextBlock.TextChanged += (sender, args) => btSave.IsEnabled = true;
        //    btSave.Click +=  BtSaveOnClick;
        //}

        //private void BtSaveOnClick(object o, RoutedEventArgs routedEventArgs)
        //{
        //    btSave.IsEnabled = false;
        //    _entry.Entry = MainTextBlock.Text;
        //    DBManager.SaveEntryChanges(_entry);
        //}
    }
}
