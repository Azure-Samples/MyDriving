// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Windows.UI.Xaml.Data;
using MyDriving.Utils;

namespace MyDriving.UWP.Converters
{
    //convert miles to km. Input is miles
    public class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "0 miles";

            double distance = (double) value;
            if (Settings.Current.MetricDistance)
            {
                double km = distance*1.60934;
                return string.Format("{0:0.00} km", km);
            }
            else
                return string.Format("{0:0.00} miles", distance);
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}