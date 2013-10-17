using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TimeTracker.Model;
using TimeTracker.util;
namespace TimeTracker.Transforms
{
    class Report
    {
        public static Report CreateWeeklyTotalsReport(Model.Model sourceModel)
        {
            ICollectionView cvs = new CollectionView(sourceModel.Entries);
            Report r = new Report(sourceModel, cvs, new WeeklyTotalsCSV());
            return r;
        }

        public static Report CreateWeeklyBillingCSVReport(Model.Model sourceModel,DateTime startDate,DateTime endDate)
        {

            ICollectionView cvs = new ListCollectionView(sourceModel.Entries);

            cvs.SortDescriptions.Add(new SortDescription("Start", ListSortDirection.Ascending));

            cvs.Filter = new Predicate<object>((o) =>
            {
                TimeEntry t = o as TimeEntry;
                if (t.Start >= startDate && t.Start < endDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            Report r = new Report(sourceModel, cvs, new ThisWeekByBillingCode());
            return r;
        }

        
        public static Report CreatingBillingReportByDay(Model.Model sourceModel,DateTime startDate,DateTime endDate)
        {
            

            ICollectionView cvs = new ListCollectionView(sourceModel.Entries);
                
            cvs.SortDescriptions.Add(new SortDescription("Start",ListSortDirection.Ascending));
            
            cvs.Filter = new Predicate<object>((o) =>
            {
                TimeEntry t = o as TimeEntry;
                if (t.Start >= startDate && t.Start < endDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            Report r=new Report(sourceModel,cvs, new BillingByDay());
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

            output += "\r\n\r\nReport generated " + DateTime.Now.ToLongDateString() + ", " + DateTime.Now.ToLongTimeString();

            return output;
        }

            
    }
}
