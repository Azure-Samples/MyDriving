// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace MyDriving.UWP.Helpers
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