using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TimeTracker.Annotations;

namespace TimeTracker.Model
{
    public class TimeEntry : INotifyPropertyChanged    
    {
        public TimeEntry()
        {
            Tags=new ObservableCollection<Tag>();
            Start = DateTime.Now;
            End = Start;

        }
        private DateTime _start;
        private DateTime _end;
        private string _note;

        public DateTime Start
        {
            get { return _start; }
            set
            {
                if (value.Equals(_start)) return;
                _start = value;
                OnPropertyChanged("Start");
            }
        }

        public DateTime End
        {
            get { return _end; }
            set
            {
                if (value.Equals(_end)) return;
                _end = value;
                OnPropertyChanged("End");
            }
        }



        public string Note
        {
            get { return _note; }
            set
            {
                if (value == _note) return;
                _note = value;
                OnPropertyChanged("Note");
            }
        }

        public ObservableCollection<Tag> Tags { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
