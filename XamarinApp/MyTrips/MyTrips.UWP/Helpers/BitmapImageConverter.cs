using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;


namespace MyTrips.UWP.Helpers
{
    public class BitmapImageConverter
    {
        //converts byte[] to BitmapImage
        public static BitmapImage ConvertImage(byte[] pictureBytes)
        {
            if (pictureBytes == null) return null;
            using (var ms = new InMemoryRandomAccessStream())
            {
                using (var writer = new DataWriter(ms.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(pictureBytes);
                    writer.StoreAsync().GetResults();
                }
                var image = new BitmapImage();
                image.SetSource(ms);
                return image;
            }
        }

    }
}
