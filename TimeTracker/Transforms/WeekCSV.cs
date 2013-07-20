using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Model;

namespace TimeTracker.Transforms
{

    public class WeekCSV : ICSVTransform
    {
        public string GetCSV(System.ComponentModel.ICollectionView source)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<DateTime, Dictionary<Tag, TimeSpan>> output = new Dictionary<DateTime, Dictionary<Tag, TimeSpan>>();
            foreach (var item in source)
            {

                var te = item as TimeEntry;
                if(!output.ContainsKey(te.Start.Date))
                {
                    output[te.Start.Date] = new Dictionary<Tag, TimeSpan>();
                }

                var dayDict=output[te.Start.Date];

                var billingTag=te.Tags.FirstOrDefault(t => t.Type == Tag.TagType.BillingCode);

                if (!dayDict.ContainsKey(billingTag))
                {
                    dayDict[billingTag]=new TimeSpan();
                }

                var tagTimeSpan= dayDict[billingTag];

                dayDict[billingTag] = tagTimeSpan.Add(te.End - te.Start);

            }

            foreach (var day in output)
            {

                foreach (var entry in day.Value)
                {
                    sb.Append(day.Key.ToShortDateString() + "," + entry.Key.PrimaryBillingCode + "," + entry.Key.SecondaryBillingCode + "," + Math.Round(entry.Value.TotalHours,3) + "\r\n");
                }
            }


            return sb.ToString();
        }
    }
}
