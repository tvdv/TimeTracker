using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TimeTracker.TimeEntryView;

namespace TimeTracker.util
{
    class GroupAggregateLengthConverter : IValueConverter, IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(values[0], targetType, parameter, culture);
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var entries = value as IEnumerable;

            var total=new TimeSpan(0);
            if (entries != null)
            {
                foreach (var obj in entries)
                {
                    var timeEntryEditViewModel = obj as TimeEntryEditViewModel;
                    total = total.Add(timeEntryEditViewModel.Length);

                }
            }

            return total.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
