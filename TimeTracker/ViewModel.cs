using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using TimeTracker.Annotations;
using TimeTracker.Model;
using TimeTracker.TimeEntryView;
using TimeTracker.util;

namespace TimeTracker
{
    class ViewModel : INotifyPropertyChanged
    {
        private readonly Model.Model _model;
        ICollectionView _timeEntryViewSource;

        private VmObservableCollection<TimeEntryEditViewModel, TimeEntry> _entryVms; 
        public ViewModel(Model.Model model)
        {
            _model = model;
            _model.PropertyChanged += _model_PropertyChanged;

            _entryVms = new VmObservableCollection<TimeEntryEditViewModel, TimeEntry>(_model.Entries, entry => new TimeEntryEditViewModel(entry, this.Tags, this.RemoveEntryCommand),
                (modelentry, vm) => vm.Model==modelentry);

            _timeEntryViewSource = CollectionViewSource.GetDefaultView(_entryVms);
            
            _timeEntryViewSource.SortDescriptions.Add(new SortDescription("Model.Start", ListSortDirection.Ascending));
            _timeEntryViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Day"));
            
        }

      

        void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                OnPropertyChanged("State");
            }
        }

        public void RefreshView()
        {
            _timeEntryViewSource.Refresh();

        }

        public ICollectionView Entries
        {
            get { return _timeEntryViewSource; }
        }

        public ObservableCollection<Tag> Tags
        {
            get { return _model.Tags; }
        }

        public ICommand AddTagCommand
        {
            get
            {
                return new RelayCommand(()=> _model.Tags.Add(new Tag()));
            }
        }


        public enum ShowEntries
        {
            All,
            Today,
            ThisWeek
        };
        public  void SetViewFilter(ShowEntries filterType)
        {
            switch(filterType)
            {
                case ShowEntries.All:
                        _timeEntryViewSource.Filter=null;
                    break;
                case ShowEntries.Today:
                    _timeEntryViewSource.Filter= new Predicate<object>(z=> ((TimeEntryEditViewModel)z).Day ==DateTime.Now.Date);
                    break;
                case ShowEntries.ThisWeek:
                    break;
            }
            
        }
        public ICommand ViewAllCommand
        {
            get
            {
                return new RelayCommand(() => { SetViewFilter(ShowEntries.All); RefreshView(); });
            }
        }
        public ICommand ViewTodayCommand
        {
            get
            {
                return new RelayCommand(() => { SetViewFilter(ShowEntries.Today); RefreshView(); });
            }
        }

        public ICommand SaveCommand
        {
            get { return new RelayCommand(()=> _model.BeginSave()); }
        }

        public ICommand AddEntryCommand
        {
            get { return new RelayCommand(() => _model.Entries.Add(new TimeEntry())); }
        }

        public ICommand RemoveEntryCommand
        {
            get
            {
                return new RelayCommand<TimeEntryEditViewModel>( e=> _model.Entries.Remove(e.Model));
            }
        }

        public Model.Model.ModelState State
        {
            get { return _model.State; }
           
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
