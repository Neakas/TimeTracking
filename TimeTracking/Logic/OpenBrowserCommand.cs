using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeTracking.Database;

namespace TimeTracking.Logic
{
    public class OpenBrowserCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string URL =
                "http://kamen.isales.de/MobileWebPortal4/SoftfolioMobilePlugin/Position/Projektvorgang?id=" +
                ((TTProjectEntry) parameter).TTProject.iksProjectID;
            Process.Start(URL);
        }

        public event EventHandler CanExecuteChanged;
    }
}
