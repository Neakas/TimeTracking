using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using FeserWard.Controls;
using TimeTracking.Database;

namespace TimeTracking.UI
{
    /// <summary>
    /// Interaction logic for NewEntry.xaml
    /// </summary>
    public partial class NewEntry : INotifyPropertyChanged
    {
        private IIntelliboxResultsProvider _linqtoentitiesprovider;

        public IIntelliboxResultsProvider LinqToEntitiesProvider
        {
            get => _linqtoentitiesprovider;
            set
            {
                _linqtoentitiesprovider = value;
                OnPropertyChanged("_linqtoentitiesprovider");
                tbKunde.DataProvider = LinqToEntitiesProvider;
            }
        }

        private TTKunde _selectedKunde;

        public TTKunde SelectedKunde
        {
            get { return _selectedKunde; }
            set { _selectedKunde = value; }
        }

        private bool open;

        public bool Open
        {
            get => open;
            set
            {
                open = value;
            }
        }



        public NewEntry()
        {
            InitializeComponent();
            SelectedKunde = null;
            LinqToEntitiesProvider = new DBManager.LinqToEntititesResultsProviderKunde(DBManager.GetKunden().ToList());
            
            tbKunde.KeyDown += (sender, e) =>
            {
                if (e.Key != Key.Return)
                {
                    return;
                }
                using (var context = new MSUtilityDBEntities())
                {
                    if (StateManager.CurrentInstance.CurrentSelectedTTDate == null)
                    {
                        TTDates date = new TTDates();
                        date.Day = StateManager.CurrentInstance.CurrentSelectedDate;
                        context.TTDates.Add(date);
                        context.SaveChanges();
                        StateManager.CurrentInstance.RefreshData();
                    }
                    object selectedItem = ((Intellibox)sender).SelectedItem;
                    if (selectedItem == null)
                    {
                        //Item existierte noch nicht. Anlegen!
                        var stringitem = ((TextBox) ((Grid) ((Intellibox) sender).Content).Children[0]).Text;
                    
                        TTKunde kunde = new TTKunde();
                        kunde.Kunde = stringitem;
                        context.TTKunde.Add(kunde);

                        SelectedKunde = kunde;
                    }
                    else
                    {
                        SelectedKunde = (TTKunde)selectedItem;
                    }

                    if (SelectedKunde != null)
                    {
                        var kundeEntry =
                            (from c in context.TTKundeEntry
                                where c.KundeID == SelectedKunde.Id &&
                                      c.DateID == StateManager.CurrentInstance.CurrentSelectedTTDate.ID
                             select c).FirstOrDefault();
                        if (kundeEntry == null)
                        {
                            kundeEntry = new TTKundeEntry();
                            kundeEntry.KundeID = SelectedKunde.Id;
                            kundeEntry.DateID = StateManager.CurrentInstance.CurrentSelectedTTDate.ID;
                            context.TTKundeEntry.Add(kundeEntry);
                        }
                        else
                        {
                            MainWindow.CurrentInstance.NewKundePopupOpen = false;
                            MainWindow.CurrentInstance.ShowGlobalMessage("Information",
                                "Kundeneintrag existiert bereits!");
                        }
                    }
                    MainWindow.CurrentInstance.NewKundePopupOpen = false;
                    ((Intellibox) sender).SelectedItem = null;
                    context.SaveChanges();
                    StateManager.CurrentInstance.RefreshData();
                    LinqToEntitiesProvider = new DBManager.LinqToEntititesResultsProviderKunde(DBManager.GetKunden().ToList());
                }
            };
            tbKunde.ResultsList.SelectionChanged += delegate
            {
                if (tbKunde.SelectedItem == null)
                {
                    return;
                }
                SelectedKunde = (TTKunde)tbKunde.SelectedValue;
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
