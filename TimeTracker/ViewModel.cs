using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using TimeTracker.Annotations;
using TimeTracker.Model;
using TimeTracker.TimeEntryView;
using TimeTracker.Transforms;
using TimeTracker.util;

namespace TimeTracker
{
    class ViewModel : INotifyPropertyChanged
    {
        private readonly Model.Model _model;
        private readonly Dispatcher _dispatcher;
        
        ListCollectionView _timeEntryViewSource;
        private ReportViewModel _reportViewModel;

        private VmObservableCollection<TimeEntryEditViewModel, TimeEntry> _entryVms; 
        public ViewModel(Model.Model model,Dispatcher dispatcher)
        {
            _reportViewModel = new ReportViewModel(model);
            _model = model;
            _dispatcher = dispatcher;
            _model.PropertyChanged += _model_PropertyChanged;

            _entryVms = new VmObservableCollection<TimeEntryEditViewModel, TimeEntry>(_model.Entries, entry => new TimeEntryEditViewModel(entry, this.Tags, this.RemoveEntryCommand),
                (modelentry, vm) => vm.Model==modelentry);

            _timeEntryViewSource = new ListCollectionView(_entryVms);

            
            _timeEntryViewSource.CustomSort = new util.TimeEntryComparer();

            _timeEntryViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Day"));

            SetViewFilter(ShowEntries.ThisWeek);
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

            System.Diagnostics.Debug.WriteLine("Refresh start" + DateTime.Now);
            _timeEntryViewSource.Refresh();
            System.Diagnostics.Debug.WriteLine("Refresh end" + DateTime.Now);
            _dispatcher.BeginInvoke(DispatcherPriority.Background,
                                    new Action(
                                        () =>
                                        System.Diagnostics.Debug.WriteLine("Refresh end - disaptcher idle" +
                                                                           DateTime.Now)));
            
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

        public ReportViewModel ReportViewModel
        {
            get
            {
                return _reportViewModel;
            }
        }

       

        public enum ShowEntries
        {
            All,
            Today,
            ThisWeek,
            LastWeek
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
                    {
                        DateTime weekStart = DateTime.Now.WeekStart();
                        var weekEnd = weekStart.AddDays(7);
                        _timeEntryViewSource.Filter = new Predicate<object>(z => ((TimeEntryEditViewModel)z).Day >= weekStart && ((TimeEntryEditViewModel)z).Day < weekEnd);
                        break;
                    }
                case ShowEntries.LastWeek:
                    {
                        DateTime weekStart = DateTime.Now.AddDays(-7).WeekStart();
                        var weekEnd = weekStart.AddDays(7);
                        _timeEntryViewSource.Filter = new Predicate<object>(z => ((TimeEntryEditViewModel)z).Day >= weekStart && ((TimeEntryEditViewModel)z).Day < weekEnd);
                        break;
                    }
            }
            
        }
        public ICommand ViewAllCommand
        {
            get
            {
                return new RelayCommand(() => { SetViewFilter(ShowEntries.All); RefreshView(); });
            }
        }



        public ICommand ViewThisWeekCommand
        {
            get
            {
                return new RelayCommand(() => { SetViewFilter(ShowEntries.ThisWeek); RefreshView(); });
            }
        }

        public ICommand ViewLastWeekCommand
        {
            get
            {
                return new RelayCommand(() => { SetViewFilter(ShowEntries.LastWeek); RefreshView(); });
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
