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
    public class TimeEntryEditViewModel : INotifyPropertyChanged, IEditableObject
    {
        private readonly TimeEntry _entry;
        private readonly ObservableCollection<Tag> _availableTags;
        private readonly ICommand _deleteEntryCommand;
        private VmObservableCollection<TimeEntryTagAssociationViewModel, Tag> _associatedTags;

        public TimeEntryEditViewModel(TimeEntry entry, ObservableCollection<Tag> availableTags,ICommand deleteEntryCommand)
        {
            _entry = entry;
            _availableTags = availableTags;
            _deleteEntryCommand = deleteEntryCommand;
            _associatedTags=new VmObservableCollection<TimeEntryTagAssociationViewModel, Tag>(entry.Tags,tag => new TimeEntryTagAssociationViewModel(tag,DeleteTagCommand),(tag, model) => model.Tag==tag );
        }

        public ICommand AddTagCommand
        {
            get
            {
                return new RelayCommand<Tag>(z=>_entry.Tags.Add(z));
            }
        }
        public ICommand DeleteTagCommand
        {
            get { return new RelayCommand<Tag>(z => _entry.Tags.Remove(z)); }
        }
        public ICommand DeleteCommand
        {
            get { return _deleteEntryCommand; }
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

        public VmObservableCollection<TimeEntryTagAssociationViewModel, Tag> AssociatedTags
        {
            get { return _associatedTags; }
        }


        public TimeSpan Length
        {
            get { return _entry.Length; }
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

        public void BeginEdit()
        {
            
        }

        public void EndEdit()
        {
            
        }

        public void CancelEdit()
        {
            
        }
    }
}
