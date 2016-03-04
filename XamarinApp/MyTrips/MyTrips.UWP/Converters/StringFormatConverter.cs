using System;
using Windows.UI.Xaml.Data;

namespace MyTrips.UWP.Converters
{
    public sealed class StringFormatConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string StringFormat = parameter as string;
            if (!String.IsNullOrEmpty(StringFormat))
                return String.Format(StringFormat, value);

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
}
