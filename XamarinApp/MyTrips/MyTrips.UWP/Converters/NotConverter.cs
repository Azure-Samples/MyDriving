using System;
using Windows.UI.Xaml.Data;

namespace MyTrips.UWP.Converters
{
    public sealed class NotConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? boolValue = value as bool?;
            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool? boolValue = value as bool?;
            return !boolValue;
        }

    }
}
