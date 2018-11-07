using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracking.Database
{

    public class StateManager : INotifyPropertyChanged
    {
        public static StateManager CurrentInstance;

        private DateTime _currentSelectedDate = DateTime.Today;
        private TTProjectEntry _currentSelectedTtProjectEntry;
        private TTKundeEntry _currentSelectedTtKundeEntry;
        private TTDates _currentSelectedTtDate;
        private TTEntryData _currentSelectedTtEntryData;

        public DateTime CurrentSelectedDate
        {
            get => _currentSelectedDate;
            set
            {
                _currentSelectedDate = value;
                CurrentSelectedTTDate = DBManager.GetDateDataFromDate(value);
                OnPropertyChanged();
            }
        }

        public TTDates CurrentSelectedTTDate
        {
            get => _currentSelectedTtDate;
            set
            {
                if (_currentSelectedTtDate != value)
                {
                    CurrentSelectedTTKundeEntry = null;
                    CurrentSelectedTTProjectEntry = null;
                    CurrentSelectedTTEntryData = null;
                }
                _currentSelectedTtDate = value;
                OnPropertyChanged();
            }
        }

        public TTKundeEntry CurrentSelectedTTKundeEntry
        {
            get => _currentSelectedTtKundeEntry;
            set
            {
                if (value == null)
                {
                    CurrentSelectedTTProjectEntry = null;
                    CurrentSelectedTTEntryData = null;
                }
                _currentSelectedTtKundeEntry = value;
                
                OnPropertyChanged();
            }
        }

        public TTProjectEntry CurrentSelectedTTProjectEntry
        {
            get => _currentSelectedTtProjectEntry;
            set
            {
                _currentSelectedTtProjectEntry = value;
                if (value != null)
                {
                    CurrentSelectedTTKundeEntry = value?.TTKundeEntry ?? null;
                    CurrentSelectedTTEntryData = value?.TTEntryData?.FirstOrDefault() ?? null;
                }

                if (value == null)
                {
                    CurrentSelectedTTEntryData = null;
                }
                OnPropertyChanged();
            }
        }

        public TTEntryData CurrentSelectedTTEntryData
        {
            get => _currentSelectedTtEntryData;
            set
            {
                _currentSelectedTtEntryData = value;
                OnPropertyChanged();
            }
        }

        public StateManager()
        {
            CurrentInstance = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RefreshData()
        {
            CurrentSelectedTTDate = DBManager.GetDateDataFromDate(CurrentSelectedDate);
        }
    }
}
