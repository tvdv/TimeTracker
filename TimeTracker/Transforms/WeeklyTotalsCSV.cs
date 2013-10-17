using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Model;

namespace TimeTracker.Transforms
{
    class WeeklyTotalsCSV : ICSVTransform
    {
        public string GetCSV(System.ComponentModel.ICollectionView source)
        {
            //DateTime is the SUNDAY at the end of the week
            //Does not account for TimeEntries that flow OVER a weekend between two weeks!
            var WeeklyTotalsBillOnly = new Dictionary<DateTime, TimeSpan>();
            var WeeklyTotals = new Dictionary<DateTime, TimeSpan>();
            foreach (var item in source)
            {
                var te = item as TimeEntry;
                var endOfWeek=GetEndOfWeek(te.Start);
                var len=te.End - te.Start;

                if (!WeeklyTotals.ContainsKey(endOfWeek))
                {
                    WeeklyTotals.Add(endOfWeek, len);
                }
                else
                {
                    WeeklyTotals[endOfWeek] = WeeklyTotals[endOfWeek].Add(len);
                }

                if (te.Tags.FirstOrDefault(z => z.Type == Tag.TagType.BillingCode) != null) //is it billable?
                {
                    if (!WeeklyTotalsBillOnly.ContainsKey(endOfWeek))
                    {
                        WeeklyTotalsBillOnly.Add(endOfWeek, len);
                    }
                    else
                    {
                        WeeklyTotalsBillOnly[endOfWeek] = WeeklyTotalsBillOnly[endOfWeek].Add(len);
                    }
                }

            }

            var sb = new StringBuilder();
            foreach (var week in WeeklyTotals)
            {
                sb.Append(week.Key.ToShortDateString() + "," + Math.Round(week.Value.TotalHours, 3) + "\r\n");
            }


            sb.AppendLine();
            sb.AppendLine("Billable time:");
            foreach (var week in WeeklyTotalsBillOnly)
            {
                sb.Append(week.Key.ToShortDateString() + "," + Math.Round(week.Value.TotalHours, 3) + "\r\n");
            }


            return sb.ToString();
        }

        

        private DateTime GetEndOfWeek(DateTime dt)
        {
            DateTime endWeek = dt.Date;
            while (endWeek.DayOfWeek != DayOfWeek.Sunday)
            {
                endWeek = endWeek.AddDays(1);
            }
            return endWeek;
        }
    }
}
