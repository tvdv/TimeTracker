using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TimeTracker.Annotations;
using TimeTracker.Model;
using TimeTracker.util;

namespace TimeTracker.TimeEntryView
{
    public class TimeEntryEditViewModel : INotifyPropertyChanged
    {
        private readonly TimeEntry _entry;
        private readonly ObservableCollection<Tag> _availableTags;

        public TimeEntryEditViewModel(TimeEntry entry, ObservableCollection<Tag> availableTags)
        {
            _entry = entry;
            _availableTags = availableTags;
        }

        public ICommand AddTagCommand
        {
            get
            {
                return new RelayCommand<Tag>(z=>_entry.Tags.Add(z));
            }
        }

        public DateTime Day
        {
            get { return _entry.Start.Date; }
            
            set { 
                _entry.Start = value + _entry.Start.TimeOfDay;
                _entry.End = value + _entry.End.TimeOfDay;
                OnPropertyChanged("Day");
            }
        }

        public TimeSpan  StartTime
        {
            get { return _entry.Start.TimeOfDay; }
            set { 
                _entry.Start = _entry.Start.Date + value;
                OnPropertyChanged("Length");
            }
        }

        public TimeSpan EndTime
        {
            get { return _entry.End.TimeOfDay; }
            set
            {
                _entry.End = _entry.End.Date + value;
                OnPropertyChanged("Length");
            }
        }

        public TimeEntry Model
        {
            get { return _entry; }
            
        }

        

        public TimeSpan Length
        {
            get { return _entry.End - _entry.Start; }
        }

        public ObservableCollection<Tag> AvailableTags
        {
            get { return _availableTags; }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
