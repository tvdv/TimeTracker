using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Model;

namespace TimeTracker.Transforms
{
    class ThisWeekByBillingCode : ICSVTransform
    {
        struct BillingCode
        {
            public string primary;
            public string secondary;
        }
        public string GetCSV(System.ComponentModel.ICollectionView source)
        {
            var sb = new StringBuilder();
            var output = new Dictionary<BillingCode, TimeSpan>();
            foreach (var item in source)
            {

                var te = item as TimeEntry;
                var billingTag = te.Tags.FirstOrDefault(t => t.Type == Tag.TagType.BillingCode);
                if (billingTag == null)
                {
                    //no billing code tag, ignore the item
                    continue;
                }

                var bc=new BillingCode {primary=billingTag.PrimaryBillingCode,secondary=billingTag.SecondaryBillingCode};
                if (!output.ContainsKey(bc))
                {
                    output[bc] = TimeSpan.FromDays(0);
                }

                var ts = output[bc];
                ts = ts.Add(te.Length);
                output[bc] = ts;
            }

            foreach (var bc in output)
            {
                sb.Append(bc.Key.primary + "," + bc.Key.secondary + "," +
                    Math.Round(bc.Value.TotalHours, 3) +
                    "\r\n");
            }


            return sb.ToString();
        }
    }
}
