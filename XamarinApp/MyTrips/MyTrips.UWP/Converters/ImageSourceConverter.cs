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

            int sourceKind = (int)value;

            if (sourceKind == (int)UserPictureSourceKind.Url)
            {
                return new BitmapImage(new Uri(Settings.Current.UserProfileUrl));
            }
            else if (sourceKind == (int)UserPictureSourceKind.Byte)
            {
                using (var ms = new InMemoryRandomAccessStream())
                {
                    var pictureBytes = (byte[])value;
                    using (var writer = new DataWriter(ms.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes(Settings.Current.UserProfileByteArr);
                        writer.StoreAsync().GetResults();
                    }
                    var image = new BitmapImage();
                    image.SetSource(ms);
                    return image;
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
