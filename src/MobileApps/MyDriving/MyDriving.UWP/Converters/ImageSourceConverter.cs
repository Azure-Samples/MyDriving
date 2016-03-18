// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace MyDriving.UWP.Converters
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

            string url = value as string;
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    return new BitmapImage(new Uri(url));
                }
                catch (Exception)
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