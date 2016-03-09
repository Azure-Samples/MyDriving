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
            var imageUrl = value as string;
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

           return new BitmapImage(new Uri(imageUrl));
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
