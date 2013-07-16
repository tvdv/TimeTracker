using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TimeTracker.Model;

namespace TimeTracker.Transforms
{
    class Report
    {
        public static Report CreateWeekCSVReport(Model.Model sourceModel)
        {
            DateTime weekStart = DateTime.Now.Date;
            while (weekStart.DayOfWeek != DayOfWeek.Monday)
            {
                weekStart = weekStart.AddDays(-1);
            }
            var weekEnd = weekStart.AddDays(7);

            ICollectionView cvs=CollectionViewSource.GetDefaultView(sourceModel.Entries);
            cvs.SortDescriptions.Add(new SortDescription("Start",ListSortDirection.Ascending));
            cvs.Filter = new Predicate<object>((o) =>
            {
                TimeEntry t = o as TimeEntry;
                if (t.Start >= weekStart && t.Start < weekEnd)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            Report r=new Report(sourceModel,cvs, new WeekCSV());
            return r;
        }
        Model.Model _sourceModel;
        ICollectionView _filter;
        ICSVTransform _transform;

        public Report(Model.Model sourceModel, ICollectionView filter, ICSVTransform transform)
        {
            _transform = transform;

            _sourceModel = sourceModel;
            _filter=filter;

        }


        public string GenerateReport()
        {
            _filter.Refresh();
            

            string output=_transform.GetCSV(_filter);
            return output;
        }

            
    }
}
