using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeTracking.Database;

namespace TimeTracking.Logic
{
    public class AddProjectCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            MainWindow.CurrentInstance.NewProjectPopupOpen = true;
            //var result = await MainWindow.CurrentInstance.ShowGlobalYesno("Achtung!",
            //    $"Sind sie sicher das sie das Relationale Object '{((dynamic)parameter).TTKunde.Kunde}' und alle Subknoten löschen wollen?");
            //if (result)
            //{
            //    using (var context = new MSUtilityDBEntities())
            //    {
            //        if (parameter.GetType() == typeof(TTKundeEntry))
            //        {
            //            var b = (TTKundeEntry)parameter;

            //            b.TTProjectEntry.ToList().ForEach(x =>
            //            {
            //                x.TTEntryData.ToList().ForEach(y =>
            //                {
            //                    context.Entry(y).State = EntityState.Deleted;
            //                });
            //                context.Entry(x).State = EntityState.Deleted;
            //            });

            //            context.Entry(b).State = EntityState.Deleted;
            //        }

            //        if (parameter.GetType() == typeof(TTProjectEntry))
            //        {
            //            var b = (TTProjectEntry) parameter;
            //            b.TTEntryData.ToList().ForEach(x =>
            //            {
            //                context.Entry(x).State = EntityState.Deleted;
            //            });
            //            context.Entry(b).State = EntityState.Deleted;
            //        }
            //        context.SaveChanges();
            //    }

            //    MainWindow.CurrentInstance.ReloadDateData();
            //}

        }

        public event EventHandler CanExecuteChanged;
    }
}
