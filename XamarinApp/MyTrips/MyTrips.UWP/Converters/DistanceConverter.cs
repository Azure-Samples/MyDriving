using System;
using System.Globalization;
using Windows.UI.Xaml.Data;
using MyTrips.Utils;


namespace MyTrips.UWP.Converters
{
    //convert miles to km. Input is miles
    public class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "0 miles";

            double distance = (double)value;
            if (Settings.Current.MetricDistance)
            {
                double km = distance * 1.60934;
                return string.Format("{0} km", (int)km);
            }
            else
                return string.Format("{0} miles", (int)distance);

        }



        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
