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
        struct DayCodeEntry
        {
            public TimeSpan Time;
            public string Notes;
        }
        public string GetCSV(System.ComponentModel.ICollectionView source)
        {
            
            StringBuilder sb = new StringBuilder();
            Dictionary<DateTime, Dictionary<Tag, DayCodeEntry>> output = new Dictionary<DateTime, Dictionary<Tag, DayCodeEntry>>();
            foreach (var item in source)
            {

                var te = item as TimeEntry;
                if(!output.ContainsKey(te.Start.Date))
                {
                    output[te.Start.Date] = new Dictionary<Tag, DayCodeEntry>();
                }

                var dayDict=output[te.Start.Date];

                var billingTag=te.Tags.FirstOrDefault(t => t.Type == Tag.TagType.BillingCode);

                if (!dayDict.ContainsKey(billingTag))
                {
                    dayDict[billingTag] = new DayCodeEntry();
                }

                var dayCodeEntry = dayDict[billingTag];
                dayCodeEntry.Time= dayCodeEntry.Time.Add(te.End - te.Start);

                if (!String.IsNullOrWhiteSpace(te.Note))
                {
                    dayCodeEntry.Notes += " " + te.Note + ".";
                }

                dayDict[billingTag] = dayCodeEntry;

            }

            foreach (var day in output)
            {

                foreach (var entry in day.Value)
                {
                    sb.Append(day.Key.ToShortDateString() + "," + entry.Key.PrimaryBillingCode + "," + entry.Key.SecondaryBillingCode + "," + 
                        Math.Round(entry.Value.Time.TotalHours,3) + 
                        ",\"" + entry.Value.Notes +  "\"" + 
                        "\r\n");
                }
            }


            return sb.ToString();
        }
    }
}
