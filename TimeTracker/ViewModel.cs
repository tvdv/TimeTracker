using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        private Model.Model.ModelState _state;

        private VmObservableCollection<TimeEntryEditViewModel, TimeEntry> _entryVms; 
        public ViewModel(Model.Model model)
        {
            _model = model;
            _model.PropertyChanged += _model_PropertyChanged;

            _entryVms=new VmObservableCollection<TimeEntryEditViewModel, TimeEntry>(_model.Entries, entry => new TimeEntryEditViewModel(entry,this.Tags),
                (modelentry, vm) => vm.Model==modelentry);

        }

      

        void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                OnPropertyChanged("State");
            }
        }
        
        public VmObservableCollection<TimeEntryEditViewModel, TimeEntry> Entries
        {
            get { return _entryVms; }
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
