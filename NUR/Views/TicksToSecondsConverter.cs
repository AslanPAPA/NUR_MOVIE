using System;
using System.Globalization;
using System.Windows.Data;

namespace NUR.Views
{
    public class TicksToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long ticks)
            {
                TimeSpan time = TimeSpan.FromTicks(ticks);

              
                if (time.TotalHours >= 1)
                {
                    return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds);
                }
                else
                {
                    return string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
                }
            }
            return "00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}