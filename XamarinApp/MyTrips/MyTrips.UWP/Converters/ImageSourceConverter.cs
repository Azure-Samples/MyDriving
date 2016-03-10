using System;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using MyTrips.Utils;

namespace MyTrips.UWP.Converters
{
    //Input value is UserPictureSourceKind
    //Actual values of the image source are pulled from Settings.Current
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            string url = value as string;
            if(!string.IsNullOrEmpty(url))
            {
                try
                {
                    return new BitmapImage(new Uri(url));
                }
                catch(Exception)
                {
                    
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
