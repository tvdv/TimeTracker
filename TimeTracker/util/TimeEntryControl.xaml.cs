using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeTracker.util
{
    /// <summary>
    /// Interaction logic for TimeEntryControl.xaml
    /// </summary>
    public partial class TimeEntryControl : UserControl
    {
        public static readonly DependencyProperty TimeFormatProperty =
            DependencyProperty.Register("TimeFormat", typeof (string), typeof (TimeEntryControl), new PropertyMetadata("hh\\:mm"));

        public string TimeFormat
        {
            get { return (string) GetValue(TimeFormatProperty); }
            set { SetValue(TimeFormatProperty, value); }
        }

        private TimeSpan lastInternalValue;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof (TimeSpan), typeof (TimeEntryControl), new PropertyMetadata(default(TimeSpan),new PropertyChangedCallback(ValuePropChanged)));

        private static void ValuePropChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var obj = dependencyObject as TimeEntryControl;
            var newVal = dependencyPropertyChangedEventArgs.NewValue is TimeSpan ? (TimeSpan) dependencyPropertyChangedEventArgs.NewValue : new TimeSpan();

            if (newVal == obj.lastInternalValue) return;

            obj.lastInternalValue = newVal;
            obj.c_txt.Text = newVal.ToString(obj.TimeFormat);
        }


        public TimeSpan Value
        {
            get { return (TimeSpan) GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
            }
        }


        public TimeEntryControl()
        {

            InitializeComponent();

            
        }

        private void C_txt_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemSemicolon && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {

                var tsNow = DateTime.Now - DateTime.Now.Date;
                c_txt.Text = tsNow.ToString(TimeFormat);
            }
            else
            {
              
            }
        }

        protected TimeSpan? ParseValue(string val)
        {
            TimeSpan outVal;
            //TODO: HACK - this destroys the ability of TimeFormat to specify everything. Fix somehow, some day
            string[] formats=new string[] {TimeFormat,"h\\:mm"};
            if (TimeSpan.TryParseExact(val, formats, CultureInfo.CurrentCulture, out outVal))
            {
                return outVal;
            }
            else
            {
                return null;
            }
        }

        private void C_txt_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = c_txt.Text;
            if (txt == "") return;

            var val = ParseValue(txt);
            if (!val.HasValue)
            {
                //not valid
                //System.Diagnostics.Debug.WriteLine("TimeEntryControl: input is invalid. Ignoring:" + txt);
            }
            else
            {
                //valid, so update binding
                lastInternalValue = val.Value;
                Value = val.Value;
                //System.Diagnostics.Debug.WriteLine("TimeEntryControl: input is valid:" +txt);
            }
        }
    }
}
