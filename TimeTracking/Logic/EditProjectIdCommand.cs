using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeTracking.Database;
using TimeTracking.UI;

namespace TimeTracking.Logic
{
    public class EditProjectIdCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            TTProjectEntry entry = (TTProjectEntry) parameter;
            EditProjectIDWindow window = new EditProjectIDWindow(entry);
            window.ShowDialog();
            StateManager.CurrentInstance.RefreshData();
        }

        public event EventHandler CanExecuteChanged;
    }
}
