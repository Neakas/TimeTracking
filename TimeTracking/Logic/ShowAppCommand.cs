using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TimeTracking.Logic
{
    public class ShowAppCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MainWindow.CurrentInstance.Visibility = Visibility.Visible;
            MainWindow.CurrentInstance.WindowState = WindowState.Normal;
        }

        public event EventHandler CanExecuteChanged;
    }
}
