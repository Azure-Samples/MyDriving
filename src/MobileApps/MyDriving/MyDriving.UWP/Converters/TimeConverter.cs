// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Windows.UI.Xaml.Data;

namespace MyDriving.UWP.Converters
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "0 hours";

            double timeSeconds = (double) value;
            TimeSpan time = TimeSpan.FromSeconds(timeSeconds);

            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s", time.Hours, time.Minutes, time.Seconds);
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}