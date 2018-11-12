using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using MSUtilityLib.Base.WPF.GlobalShortCut;
using TimeTracking.Database;
using TimeTracking.Logic;
using TimeTracking.UI;
using Brushes = System.Windows.Media.Brushes;

namespace TimeTracking
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public static MainWindow CurrentInstance;

        private List<DateTime> _calenderEntries;

        private bool _newKundePopupOpen;

        private bool _newProjectPopupOpen;

        public ApplicationShortCutManager AppShortCutManager, WindowShortCutManager;

        public SystemShortCutManager scm;

        public StateManager StateManager { get; set; } = new StateManager();
        public WindowState LastState;

        #region ShowBusyIndicator Property
        private bool _showBusyIndicator;
        public bool ShowBusyIndicator
        {
            get => _showBusyIndicator; 
            set
            {
                if (_showBusyIndicator == value)
                    return;

                _showBusyIndicator = value; 
                OnPropertyChanged();
            }
        }
        #endregion ShowBusyIndicator Property

        public MainWindow()
        {
            //Der Kommentar war nicht so Toll!
            TaskbarIcon icon = new TaskbarIcon();
            icon.Icon = new Icon("favicon.ico");
            icon.DoubleClickCommand = new ShowAppCommand();
            
            icon.ToolTipText = "Time-Tracking";


            this.StateChanged += (sender, args) =>
            {
                if (WindowState == WindowState.Minimized)
                {
                    this.ShowInTaskbar = false;
                }
                else
                {
                    this.ShowInTaskbar = true;
                }
            };
            CurrentInstance = this;
            InitializeComponent();
            AppShortCutManager = new ApplicationShortCutManager(tbProjectText);
            WindowShortCutManager = new ApplicationShortCutManager(this);

            AppShortCutManager.AddItem(new ApplicationShortCutItem(Key.S, ModifierKeys.Control), OnSaveText);
            WindowShortCutManager.AddItem(new ApplicationShortCutItem(Key.Escape, ModifierKeys.None), OnEscape);

            scm = new SystemShortCutManager(this);
            scm.AddItem(new SystemShortCutItem(Key.Add,ModifierKeys.Control), OnOpen);
           
            SetupCalender();
        }

        private void OnOpen(object sender, EventArgs e)
        {
            this.Show();
            this.Focus();
            this.Activate();
            this.WindowState = WindowState.Normal;
        }

        public bool NewKundePopupOpen
        {
            get => _newKundePopupOpen;
            set
            {
                _newKundePopupOpen = value;
                popupNewEntry.Open = value;
                OnPropertyChanged();
            }
        }

        public bool NewProjectPopupOpen
        {
            get => _newProjectPopupOpen;
            set
            {
                _newProjectPopupOpen = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnSaveText(object sender, EventArgs eventArgs)
        {
            using (var context = new MSUtilityDBEntities())
            {
                if (StateManager.CurrentSelectedTTEntryData != null)
                {
                    StateManager.CurrentSelectedTTEntryData.Content = tbProjectText.Text;
                    context.Entry(StateManager.CurrentSelectedTTEntryData).State = EntityState.Modified;
                }
                else
                {
                    var currentEntryData =
                        new TTEntryData
                        {
                            ProjectEntryId = StateManager.CurrentSelectedTTProjectEntry.Id,
                            Content = tbProjectText.Text,
                            Header = StateManager.CurrentSelectedTTProjectEntry.TTProject.Project
                        };
                    context.TTEntryData.Add(currentEntryData);
                }
                context.SaveChanges();
                StateManager.CurrentInstance.RefreshData();
            }
        }

        private void OnEscape(object sender, EventArgs eventArgs)
        {
            if (this.WindowState != WindowState.Minimized)
                this.WindowState = WindowState.Minimized;
        }
        private void SetupCalender()
        {
            mainCalenderOnDisplayDateChanged = OnDisplayDateChanged;
            MainCalender.DateHighlightBrush = Brushes.LightGreen;
            RefreshCalender();
            MainCalender.SelectedDatesChanged += MainCalenderOnSelectedDatesChanged;
            MainCalender.DisplayDateChanged += mainCalenderOnDisplayDateChanged;
            MainCalender.SelectedDate = DateTime.Today;
            SelectFirstEntry();
        }

        private EventHandler<CalendarDateChangedEventArgs> mainCalenderOnDisplayDateChanged;

        public void RefreshCalender()
        {
            _calenderEntries = DBManager.GetCalenderEntries();
            ResetCalenderHighlight();
            FillCalenderHighlighting(_calenderEntries);
            MainCalender.Refresh();
        }

        private void FillCalenderHighlighting(List<DateTime> ttDates)
        {
            if (ttDates.Count == 0) return;
            ttDates.ForEach(x =>
            {
                MainCalender.HighlightedDateText[x.Day - 1] = "!";
            });
        }


        private void OnDisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            if (e.AddedDate == null) return;
            if (e.AddedDate.Value.Year == 1) return;
            var newyear = e.AddedDate.Value.Year;
            var newmonth = e.AddedDate.Value.Month;
            var needRefresh = StateManager.CurrentSelectedDate.Year != newyear || StateManager.CurrentSelectedDate.Month != newmonth;
            StateManager.CurrentSelectedDate = e.AddedDate.Value;

            if (needRefresh) RefreshCalender();
        }
        private void MainCalenderOnSelectedDatesChanged(object sender,SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var day = (DateTime) selectionChangedEventArgs.AddedItems[0];
            if (StateManager.CurrentSelectedDate.Month == day.Month && StateManager.CurrentSelectedDate.Year == day.Year)
            {
                StateManager.CurrentSelectedDate = day;
                SelectFirstEntry();
            }
            
        }

        /// <summary>
        /// Selektiert den ersten Zeiteintrag, sodass sofort der Erfassungstext angezeigt wird
        /// </summary>
        private void SelectFirstEntry()
        {
            // Parent-Node
            if (treeView.ItemContainerGenerator.Items.Count == 0)
                return;
                if (!(treeView.ItemContainerGenerator.Items[0] is TTKundeEntry node))
                return;

            StateManager.CurrentSelectedTTProjectEntry = node.TTProjectEntry.First();
        }

        private void ResetCalenderHighlight()
        {
            for (var i = 0; i < 31; i++) MainCalender.HighlightedDateText[i] = null;
        }

        //Show Global Messsage Async
        public async Task ShowGlobalMessage(string title, string message)
        {
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                var setting = new MetroDialogSettings {ColorScheme = MetroDialogColorScheme.Accented};

                await this.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, setting);
            });
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ButtonNewKunde_Click(object sender, RoutedEventArgs e) => NewKundePopupOpen = true;

        private void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            ShowBusyIndicator = true; // schon hier, wegen SaveFileDialog, der dafür sorgt, dass das UI direkt refresht wird

            //JL: int year = MainCalender.DisplayDate.Year, month = MainCalender.DisplayDate.Month;
            int year = StateManager.CurrentSelectedDate.Year, month = StateManager.CurrentSelectedDate.Month;

            var sfd = new SaveFileDialog
            {
                Filter = "Excel-Datei|*.xlsx",
                FileName = $"TimeTracking_Export_{year}_{month:00}.xlsx"
            };

            if (sfd.ShowDialog(this) != true)
            {
                ShowBusyIndicator = false;
                return;
            }

            #region Export-Logic with BackgroundWorker
            var bw = new BackgroundWorker();

            bw.DoWork += (obj, args) =>
                ExcelHelper.ExportTimeEntries(
                    month, 
                    year, 
                    ExcelExportOptions.SaveAndOpen(new FileInfo(sfd.FileName)));

            bw.RunWorkerCompleted += (obj, args) => ShowBusyIndicator = false;

            bw.RunWorkerAsync();
            #endregion Export-Logic with BackgroundWorker
        }

        public async Task<bool> ShowGlobalYesno(string title, string message)
        {
            return await Application.Current.Dispatcher.Invoke(async () =>
            {
                var setting = new MetroDialogSettings
                {
                    AffirmativeButtonText = "Ja",
                    NegativeButtonText = "Nein",
                    ColorScheme = MetroDialogColorScheme.Accented
                };

                var result = await this.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative,
                    setting);
                if (result == MessageDialogResult.Affirmative) return true;

                return false;
            });
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null) return;
            if (e.NewValue.GetType() == typeof(TTProjectEntry))
            {
                StateManager.CurrentSelectedTTProjectEntry = (TTProjectEntry)e.NewValue;
            }

            if (e.NewValue.GetType() == typeof(TTKundeEntry))
            {
                StateManager.CurrentSelectedTTKundeEntry = (TTKundeEntry)e.NewValue;
                popupProject.LinqToEntitiesProvider = new DBManager.LinqToEntititesResultsProviderProject(DBManager.GetProjects(StateManager.CurrentInstance.CurrentSelectedTTKundeEntry).ToList());
                StateManager.CurrentSelectedTTProjectEntry = null;
                
            }
        }
    }
}