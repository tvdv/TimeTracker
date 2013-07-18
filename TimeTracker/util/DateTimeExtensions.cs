using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.util
{
    public static class DateTimeExtensions
    {
        public static DateTime WeekStart(this DateTime d)
        {
            var weekStart = d.Date;
            while (weekStart.DayOfWeek != DayOfWeek.Monday)
            {
                weekStart = weekStart.AddDays(-1);
            }
            return weekStart;
        }
    }
}
