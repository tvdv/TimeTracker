using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Model;
using TimeTracker.TimeEntryView;

namespace TimeTracker.util
{
    class TimeEntryComparer : Comparer<TimeEntryView.TimeEntryEditViewModel>
    {
        public override int Compare(TimeEntryEditViewModel x, TimeEntryEditViewModel y)
        {
            return DateTime.Compare(x.Model.Start, y.Model.Start);
        }
    }
}
