using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeTracker.Annotations;
using TimeTracker.Transforms;
using TimeTracker.util;


namespace TimeTracker
{
    class ReportViewModel : INotifyPropertyChanged
    {
        private Model.Model _model;
        public ReportViewModel(Model.Model model)
        {
            _model = model;
            StartDate = DateTime.Now.WeekStart();
            EndDate = StartDate.AddDays(7);
        }
        string _lastCSVReport;
        public ICommand ByBillingCodeCommand
        {
            get
            {
                return new RelayCommand(() => { var r = Report.CreateWeeklyBillingCSVReport(_model,StartDate,EndDate); _lastCSVReport = r.GenerateReport(); OnPropertyChanged("LastCSVReport"); });
            }

        }

        public ICommand ByDayCommand
        {
            get
            {
                return new RelayCommand(() => { var r = Report.CreatingBillingReportByDay(_model,StartDate,EndDate); _lastCSVReport = r.GenerateReport(); OnPropertyChanged("LastCSVReport"); });
            }

        }

        public ICommand WeeklyTotalsCSVCommand
        {
            get
            {
                return new RelayCommand(() => { var r = Report.CreateWeeklyTotalsReport(_model); _lastCSVReport = r.GenerateReport(); OnPropertyChanged("LastCSVReport"); });
            }

        }

        public String LastCSVReport
        {
            get { return _lastCSVReport; }
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
