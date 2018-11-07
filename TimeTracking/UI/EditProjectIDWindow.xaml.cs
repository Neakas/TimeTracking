using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TimeTracking.Database;

namespace TimeTracking.UI
{
    /// <summary>
    /// Interaction logic for EditProjectIDWindow.xaml
    /// </summary>
    public partial class EditProjectIDWindow : INotifyPropertyChanged
    {
        private TTProjectEntry _entry;

        public TTProjectEntry Entry
        {
            get { return _entry; }
            set
            {
                _entry = value;
                OnPropertyChanged();
            }
        }

        private int _posId;

        public int PosId
        {
            get { return _posId; }
            set
            {
                _posId = value; 
                OnPropertyChanged();
            }
        }


        public EditProjectIDWindow(TTProjectEntry entry)
        {
            InitializeComponent();
            Entry = entry;
            PosId = Entry.TTProject.iksProjectID ?? 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new MSUtilityDBEntities())
            {
                Entry.TTProject.iksProjectID = PosId;
                context.Entry(Entry.TTProject).State = EntityState.Modified;
                context.SaveChanges();
            }
            this.Close();
        }
    }
}
