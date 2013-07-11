using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TimeTracker.util
{
    class DateTimeToFriendlyDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = (DateTime) value;

            var today = DateTime.Now.Date;
            if (dt.Date == today)
            {
                return "Today (" + GetDateStr(dt) + ")";
            }
            else if (dt.Date == today.AddDays(-1))
            {
                return "Yesterday (" + GetDateStr(dt) + ")";
            }
            else
                return GetDateStr(dt);
        }

        protected string GetDateStr(DateTime dt)
        {
            return dt.ToLongDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
