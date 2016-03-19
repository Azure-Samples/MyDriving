// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Windows.UI.Xaml.Data;

namespace MyDriving.UWP.Converters
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